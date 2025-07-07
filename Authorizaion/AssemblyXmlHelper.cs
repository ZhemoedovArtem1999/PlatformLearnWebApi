using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Authentication
{
    // TODO: вынести в ядро в будущем
    internal static class AssemblyXmlHelper
    {
        /// <summary>
        /// Проверяет что переданный путь содержит xml-файл с документацией ().
        /// </summary>
        /// <param name="filePath">путь к файлу с документацией.</param>
        /// <returns>Передает <see langword="true"/> если файл корректный.</returns>
        public static bool IsAssemblyDocXml(string filePath)
        {
            try
            {
                if (File.Exists(filePath) && filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    var doc = XDocument.Load(filePath);

                    return doc.Root?.Name == "doc" && doc.Root.Element("members") is not null;
                }
            }
            catch
            {
            }

            return false;
        }
    }
}
