using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extractor.Models.DBPostgresSQL;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        await ExtractDataAndSaveToPostgresSQLAsync(folderPath);
        
        time.Stop();
        Console.WriteLine($"Tiempo total: {time.Elapsed.TotalMilliseconds} ms");
        Console.WriteLine("Datos extraídos y guardados");
    }
    
    static async Task ExtractDataAndSaveToPostgresSQLAsync(string folderPath)
    {
        using (var context = new AppDbContext()){
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
                            Cik = cik,
                            Entityname = entityname 
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

    static async Task TickerFromJsontoPostgres(){
        // // "\d+"\s*:\s*  expresion regular para eliminar cosas innecesarias en el json
        // var jsonFilePath = "Tickers/Tickers.json";

        // var jsonString = await File.ReadAllTextAsync(jsonFilePath);
        // var options = new System.Text.Json.JsonSerializerOptions
        // {
        //     PropertyNameCaseInsensitive = true
        // };
        // var tickers = System.Text.Json.JsonSerializer.Deserialize<List<Ticker>>(jsonString, options);
        // using (var dbContext = new AppDbContext())
        // {
        //     dbContext.Database.EnsureCreated();
        //     if (tickers != null)
        //     {
        //         await dbContext.Tickers.AddRangeAsync(tickers);
        //         await dbContext.SaveChangesAsync();
        //         Console.WriteLine(num);
        //         num += 1;
        //     }
        // }






        
        // "\d+"\s*:\s*  expresion regular para eliminar cosas innecesarias en el json
        var jsonFilePath = "Tickers/Tickers.json";
        var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        var jsonString = await File.ReadAllTextAsync(jsonFilePath);
        var data = System.Text.Json.JsonSerializer.Deserialize<List<Ticker>>(jsonString, options);
        var uniqueTickers = data?.GroupBy(t => t.CikStr).Select(g => g.First()).ToList();
        Console.WriteLine(data?.ElementAt(0).CikStr);

        using (var dbContext = new AppDbContext())
        {
            foreach (Ticker item in data)
            {
                dbContext.Tickers.Add(item);
            }
            await dbContext.SaveChangesAsync();
            Console.WriteLine(num);
            num += 1;
        }
    }




























    static async Task ExtractDataAndSaveToSQLiteAsync(string folderPath)
    {
        using (var context = new Extractor.Models.DBSQLite.SQLiteDbContext()){
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
                        Extractor.Models.DBSQLite.Company company = new Extractor.Models.DBSQLite.Company
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
