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
        
        string folderPath = @"companyfacts";
        string outputFile = @"companys.json";

        await ExtractDataAndSaveToJsonAsync(folderPath, outputFile);
        
        time.Stop();
        Console.WriteLine($"Tiempo total: {time.Elapsed.TotalMilliseconds} ms");
        Console.WriteLine($"Datos extraídos y guardados en {outputFile}");
    }

    static async Task ExtractDataAndSaveToJsonAsync(string folderPath, string outputFile)
    {
        using StreamWriter streamWriter = new StreamWriter(outputFile);
        
        string[] filePaths = Directory.GetFiles(folderPath, "*.json");
        await streamWriter.WriteLineAsync("[");
        foreach (string filePath in filePaths)
        {
            try
            {
                Console.WriteLine($"Procesando archivo: {filePath}");

                string jsonContent = await File.ReadAllTextAsync(filePath);
                JObject jsonData = JObject.Parse(jsonContent);

                string cik = filePath.Substring(13,13);
                string entityName = jsonData["entityName"]?.ToString();

                if (!string.IsNullOrEmpty(cik) && !string.IsNullOrEmpty(entityName))
                {
                    var dataItem = new
                    {
                        cik,
                        entityName = entityName 
                    };

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
