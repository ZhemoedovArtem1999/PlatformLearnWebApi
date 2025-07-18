import fs from "node:fs";
import path from "node:path";
import { generateApi } from "swagger-typescript-api";

const outputPath = path.resolve(process.cwd(), "./src/api");

generateApi({
  url: "http://localhost:5206/swagger/v1/swagger.json",
  output: outputPath, // куда складывать сгенерированные файлы. Для нас бесполезна потому что мы сохраняем файлы вручную 
  cleanOutput: false, // отчистка output перед сохранением новых файлов. Для нас бесполезна потому что мы сохраняем файлы вручную 
   templates: outputPath + "/templates",
  // generateUnionEnums: true,
  extractEnums: true,
  sortTypes: true,
  sortRoutes: true,
  modular: true,
  extraTemplates: [{name: "index", path: outputPath + "/templates/index.ejs"}],
  unwrapResponseData: true, // если true, то api возвращает Promise<T> или выбрасывает ошибку E. Если false, то api возвращает Promise<HttpResponse<T, E>>.
  moduleNameFirstTag: true, // разделение api на отдельные классы в зависимости от тега (как разделено на странице swagger)
})
.then(output => {

  fs.rmSync(outputPath + "/api", {force: true, recursive: true });
  
  const files = fs.readdirSync(outputPath);
        
  for (const file of files) {
      if (file.endsWith(".ts") || (file != "templates" && file != "build-api.tsx" && file != "index.tsx" ))
      {
        const filePath = path.join(outputPath, file);
        fs.unlinkSync(filePath);
      }
  }

  fs.mkdirSync(outputPath + "/api");

  output.files.forEach(fileInfo => {
      if (fileInfo.fileName == "index")
      {
        fs.writeFileSync(outputPath + "/" + fileInfo.fileName + ".tsx", fileInfo.fileContent);
      }
      else  {
        let fileName = fileInfo.fileName;

        if (fileName != "data-contracts" && fileName != "http-client")
          fileName += "Api.tsx";
        else 
          fileName += ".tsx";

        fs.writeFileSync(outputPath + "/api/" + fileName, fileInfo.fileContent);
      }
})})
.catch((e) => console.error(e));