using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extractor.Models.DBSQLite;
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

        await ExtractDataAndSaveToSQLiteAsync(folderPath);
        
        time.Stop();
        Console.WriteLine($"Tiempo total: {time.Elapsed.TotalMilliseconds} ms");
        Console.WriteLine("Datos extraídos y guardados");
    }

    static async Task ExtractDataAndSaveToSQLiteAsync(string folderPath)
    {
        using (var context = new SQLiteDbContext()){
            string[] filePaths = Directory.GetFiles(folderPath, "*.json");
            foreach (string filePath in filePaths)
            {
                try
                {
                    Console.WriteLine($"Procesando archivo: {filePath}");

                    string jsonContent = await File.ReadAllTextAsync(filePath);
                    JObject jsonData = JObject.Parse(jsonContent);

                    string cik = filePath.Substring(13,13);
                    string entityname = jsonData["entityName"]?.ToString();

                    if (!string.IsNullOrEmpty(cik) && !string.IsNullOrEmpty(entityname))
                    {
                        Company company = new Company
                        {
                            CIK = cik,
                            entityName = entityname 
                        };
                        await context.AddAsync(company);
                        await context.SaveChangesAsync();
                        Console.WriteLine(num);
                        num += 1;
                    }
                }
                catch (JsonException)
                {
                    Console.WriteLine($"Error decodificando JSON del archivo: {filePath}");
                }
            }
        }
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
