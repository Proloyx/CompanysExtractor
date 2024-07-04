using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    static int num = 0;
    static async Task Main()
    {
        Stopwatch time = new Stopwatch();
        time.Start();
        
        string folderPath = @"companyfacts"; // Reemplaza con la ruta a tu carpeta
        string outputFile = @"company.json";

        await ExtractDataAndSaveToJsonAsync(folderPath, outputFile);
        
        time.Stop();
        Console.WriteLine($"Tiempo total: {time.Elapsed.TotalMilliseconds} ms");
        Console.WriteLine($"Datos extraídos y guardados en {outputFile}");
    }

    static async Task ExtractDataAndSaveToJsonAsync(string folderPath, string outputFile)
    {
        using StreamWriter streamWriter = new StreamWriter(outputFile); // Abre un StreamWriter para escribir en el archivo
        
        string[] filePaths = Directory.GetFiles(folderPath, "*.json");
        await streamWriter.WriteLineAsync("[");
        foreach (string filePath in filePaths)
        {
            try
            {
                Console.WriteLine($"Procesando archivo: {filePath}");

                string jsonContent = await File.ReadAllTextAsync(filePath);
                JObject jsonData = JObject.Parse(jsonContent);

                string cik = filePath.Substring(14,13);
                string entityName = jsonData["entityName"]?.ToString();

                if (!string.IsNullOrEmpty(cik) && !string.IsNullOrEmpty(entityName))
                {
                    // Crear un objeto anónimo con los datos requeridos
                    var dataItem = new
                    {
                        cik,
                        entityName = entityName // Renombrar según la convención de C# para evitar espacios
                    };

                    // Serializar el objeto anónimo directamente al archivo
                    string jsonLine = JsonConvert.SerializeObject(dataItem) + ",";
                    await streamWriter.WriteLineAsync(jsonLine);
                    Console.WriteLine(num);
                    num += 1;
                }
            }
            catch (JsonException)
            {
                Console.WriteLine($"Error decodificando JSON del archivo: {filePath}");
            }
        }
        await streamWriter.WriteLineAsync("]");
    }
}
