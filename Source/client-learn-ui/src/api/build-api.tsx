import fs from "node:fs";
import path from "node:path";
import { generateApi } from "swagger-typescript-api";

const OUTPUT_PATH = path.resolve(process.cwd(), "./src/api");
const TEMPLATES_PATH = path.join(OUTPUT_PATH, "templates");
const GATEWAY_URL = "http://localhost:5206";

async function main() {
  try {
    // Генерация API из Swagger

    
    const apiOutput = await generateApi({
      url: `${GATEWAY_URL}/swagger/v1/swagger.json`,
      output: OUTPUT_PATH,
      cleanOutput: false, // отчистка output перед сохранением новых файлов. Для нас бесполезна потому что мы сохраняем файлы вручную
      templates: TEMPLATES_PATH,
      // generateUnionEnums: true,
      extractEnums: true,
      sortTypes: true,
      sortRoutes: true,
      modular: true,
      extraTemplates: [
        { name: "index", path: OUTPUT_PATH + "/templates/index.ejs" },
      ],
      unwrapResponseData: true, // если true, то api возвращает Promise<T> или выбрасывает ошибку E. Если false, то api возвращает Promise<HttpResponse<T, E>>.
      moduleNameFirstTag: true,
    });

    // Очистка и подготовка директории
    fs.rmSync(OUTPUT_PATH + "/api", { force: true, recursive: true });

    const files = fs.readdirSync(OUTPUT_PATH);
    for (const file of files) {
      if (
        file.endsWith(".ts") ||
        (file !== "templates" &&
          file != "protos" &&
          file !== "build-api.tsx" &&
          file !== "index.tsx" &&
          file != "grpc")
      ) {
        fs.unlinkSync(path.join(OUTPUT_PATH, file));
      }
    }

    fs.mkdirSync(OUTPUT_PATH + "/api");

    // Сохранение сгенерированных файлов API
    apiOutput.files.forEach((fileInfo) => {
      if (fileInfo.fileName == "index") {
        fs.writeFileSync(
          OUTPUT_PATH + "/" + fileInfo.fileName + ".tsx",
          fileInfo.fileContent
        );
      } else {
        let fileName = fileInfo.fileName;

        if (fileName != "data-contracts" && fileName != "http-client")
          fileName += "Api.tsx";
        else fileName += ".tsx";

        fs.writeFileSync(
          OUTPUT_PATH + "/api/" + fileName,
          fileInfo.fileContent
        );
      }
    });

    console.log("Генерация API и gRPC клиентов завершена успешно");
  } catch (e) {
    console.error("Ошибка во время генерации:", e);
    process.exit(1);
  }
}

main();
