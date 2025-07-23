import {
  rmSync,
  writeFileSync,
  existsSync,
  mkdirSync,
  readdirSync,
  readFileSync,
} from "fs";
import path from "path";
import { execSync } from "child_process";

const OUTPUT_PATH = path.resolve(process.cwd(), "./src/api");
const PROTO_DIR = path.join(OUTPUT_PATH, "protos");
const OUT_DIR_GRPC = path.join(OUTPUT_PATH, "grpc");
const OUT_DIR_GRPC_API = path.join(OUT_DIR_GRPC, "api");
const OUT_DIR_GRPC_CLIENT = path.join(OUT_DIR_GRPC, "client");

const CLIENT_TEMPLATE = (
  api:string,
  serviceName: string,
  fileName: string,
  protoImportPath: string,
  methods: string[],
  messageInterfaces: string
) => `
import { ${serviceName} } from '../api/${path.basename(
  fileName,
  "_grpc_web_pb.js"
)}/${fileName}';
import * as grpcWeb from 'grpc-web';
import * as api from '../api/${path.basename(
  fileName,
  "_grpc_web_pb.js"
)}/${protoImportPath.replace("_pb.d", "_pb")}';
const GATEWAY_HOST = process.env.REACT_APP_GATEWAY_HOST + '/${api}';

${messageInterfaces}

export class ${serviceName.replace(/Client$/, "")}Service {
  private client: ${serviceName};
  private metadata: { [key: string]: string } = {};

  ${methods
    .map(
      (m) => `
      
       public ${m} = async (request: api.${m}Request): Promise<I${m}Response> => {
       try { 
        const response = await this.${m}Internal(request);
        return response as unknown as I${m}Response;
      }
        catch(error){
        if (error instanceof grpcWeb.RpcError){
          throw error;
        }
          else{
            throw error;
            }
        }
    
  };

  private ${m}Internal = (request: api.${m}Request): Promise<api.${m}Response> => {
    return this.callMethod('${
      m.charAt(0).toLowerCase() + m.slice(1)
    }', request);
  };`
    )
    .join("")}

  constructor() {
    this.client = new ${serviceName}(GATEWAY_HOST, null, {
      format: 'binary',
      withCredentials: false,
      debug: process.env.NODE_ENV === 'development'
    });
  }

  setMetadata(metadata: { [key: string]: string }) {
    this.metadata = { ...metadata };
  }

  addMetadata(key: string, value: string) {
    this.metadata[key] = value;
  }

  getMetadata(): grpcWeb.Metadata {
    const metadata: grpcWeb.Metadata = {};
    Object.entries(this.metadata).forEach(([key, value]) => {
      metadata[key] = value;
    });
    return metadata;
  }

  clearMetadata() {
    this.metadata = {};
  }

  private async callMethod<TRequest, TResponse>(
    methodName: string,
    request: TRequest
  ): Promise<TResponse> {
    return new Promise((resolve, reject) => {
      const method = (this.client as any)[methodName];
      if (!method) {
        reject(new Error(\`Method \${methodName} not found\`));
        return;
      }

      method.call(
        this.client,
        request,
        this.getMetadata(),
        (err: grpcWeb.RpcError, response: TResponse) => {
          if (err) {
            reject(err);
          } else {
            resolve(
              typeof (response as any).toObject === 'function'
                ? (response as any).toObject()
                : response
            );
          }
        }
      );
    });
  }
}

export const ${serviceName.replace(
  /Client$/,
  ""
)}ClientApi = new ${serviceName.replace(/Client$/, "")}Service();
`;

async function generateProto() {
  try {
    if (existsSync(OUT_DIR_GRPC)) {
      rmSync(OUT_DIR_GRPC, { recursive: true, force: true });
    }
    mkdirSync(OUT_DIR_GRPC_API, { recursive: true });

    const protoFiles = readdirSync(PROTO_DIR)
      .filter((file) => file.endsWith(".proto"))
      .map((file) => path.join(PROTO_DIR, file));

    if (protoFiles.length === 0) {
      throw new Error("No .proto files found in " + PROTO_DIR);
    }

    const protocPath =
      process.platform === "win32"
        ? '"C:\\Program Files\\protoc\\bin\\protoc.exe"'
        : "protoc";

    const grpcWebPlugin = path.join(
      process.cwd(),
      "node_modules",
      ".bin",
      process.platform === "win32"
        ? "protoc-gen-grpc-web.cmd"
        : "protoc-gen-grpc-web"
    );

    for (const protoFile of protoFiles) {
      const protoFileName = path.basename(protoFile, ".proto");
      const outDirWithProtoName = path.join(OUT_DIR_GRPC_API, protoFileName);
      mkdirSync(outDirWithProtoName, { recursive: true });

      const command = [
        protocPath,
        `--plugin=protoc-gen-grpc-web="${grpcWebPlugin}"`,
        `--grpc-web_out=import_style=commonjs+dts,mode=grpcwebtext:${outDirWithProtoName}`,
        `--js_out=import_style=commonjs,binary:${outDirWithProtoName}`,
        `--proto_path=${PROTO_DIR}`,
        `"${protoFile}"`,
      ].join(" ");

      execSync(command, { stdio: "inherit", shell: true });
      await generateClientFiles(protoFileName, outDirWithProtoName);
    }

    console.log("Proto files generated successfully");
  } catch (error) {
    console.error("Proto generation failed:", error);
    process.exit(1);
  }
}

async function generateClientFiles(fileName: string, outDir: string) {
  try {
    console.log("Generating client files...");
    mkdirSync(OUT_DIR_GRPC_CLIENT, { recursive: true });

    const generatedFiles = readdirSync(outDir);

    const clientFiles = generatedFiles.filter(
      (file) =>
        file.endsWith("ServiceClientPb.js") || file.endsWith("_grpc_web_pb.js")
    );

    if (clientFiles.length === 0) {
      throw new Error("No client files found in generated files");
    }

    for (const clientFile of clientFiles) {
      const serviceName = clientFile.endsWith("_grpc_web_pb.js")
        ? path.basename(clientFile, "_grpc_web_pb.js") + "ServiceClient"
        : path.basename(clientFile, "ServiceClientPb.js") + "Client";

      const baseName = serviceName.replace("Client", "").replace("Service", "");

      const protoFile = generatedFiles.find(
        (f) =>
          f === `${baseName.toLowerCase()}_pb.js` || f === `${baseName}_pb.js`
      );

      if (!protoFile) {
        console.warn(`No proto file found for ${baseName}`);
        continue;
      }

      // Читаем proto-файл для извлечения методов
      const protoContent = readFileSync(
        path.join(PROTO_DIR, `${baseName.toLowerCase()}.proto`),
        "utf-8"
      );
      const methods = extractMethodsFromProto(protoContent);
      const messageInterfaces = generateMessageInterfaces(protoContent);

      const clientFileName = `${baseName.toLowerCase()}.client.ts`;
      const clientFilePath = path.join(OUT_DIR_GRPC_CLIENT, clientFileName);

      writeFileSync(
        clientFilePath,
        CLIENT_TEMPLATE(
          baseName,
          serviceName.charAt(0).toUpperCase() + serviceName.slice(1),
          clientFile.replace(".ts", ""),
          protoFile.replace(".ts", ""),
          methods,
          messageInterfaces
        )
      );
      console.log(`Generated client: ${clientFileName}`);
    }
  } catch (error) {
    console.error("Client generation failed:", error);
  }
}

// Функция для извлечения методов из proto-файла
function extractMethodsFromProto(protoContent: string): string[] {
  const serviceMatch = protoContent.match(/service\s+\w+\s*{([^}]*)}/);
  if (!serviceMatch) return [];

  const methodsGrpc = [];
  const methodRegex = /rpc\s+(\w+)\s*\([^)]*\)\s*returns\s*\([^)]*\)/g;
  let match;

  while ((match = methodRegex.exec(serviceMatch[1]))) {
    methodsGrpc.push(match[1]);
  }

  return methodsGrpc;
}

// Функция для генерации интерфейсов из proto-файла
function generateMessageInterfaces(protoContent: string): string {
  const messageRegex = /message\s+(\w+)\s*{([^}]*)}/g;
  let match;
  const interfaces: string[] = [];

  while ((match = messageRegex.exec(protoContent))) {
    const messageName = match[1];
    const fields = match[2].trim();
    const fieldLines = fields
      .split("\n")
      .filter((line) => line.trim().length > 0);

    const interfaceFields = fieldLines
      .map((line) => {
        const fieldMatch = line.trim().match(/(\w+)\s+(\w+)\s*=\s*\d+/);
        if (!fieldMatch) return "";

        const type = convertProtoTypeToTs(fieldMatch[1]);
        const name = fieldMatch[2];
        return `  ${name}: ${type};`;
      })
      .filter(Boolean)
      .join("\n");

    interfaces.push(`export interface I${messageName} {
${interfaceFields}
}`);
  }

  return interfaces.join("\n\n");
}

// Конвертер proto-типов в TypeScript
function convertProtoTypeToTs(protoType: string): string {
  const typeMap: Record<string, string> = {
    string: "string",
    bool: "boolean",
    int32: "number",
    int64: "number",
    uint32: "number",
    uint64: "number",
    float: "number",
    double: "number",
    bytes: "Uint8Array",
    Timestamp: "Date",
  };

  if (protoType.startsWith("repeated")) {
    const innerType = protoType.replace("repeated", "").trim();
    return `${convertProtoTypeToTs(innerType)}[]`;
  }
  return typeMap[protoType] || protoType;
}

generateProto().catch(console.error);
