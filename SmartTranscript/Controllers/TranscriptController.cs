using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartTranscript.Models;

namespace SmartTranscript.Controllers
{
    public class TranscriptController : Controller
    {
        private readonly ILogger<TranscriptController> _logger;

        public TranscriptController(ILogger<TranscriptController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Index(string urlName, string videoId)
        {
            //string apiUrl = "https://euno-1.api.microsoftstream.com/api/videos/fa51e2eb-175e-4c72-97aa-d5c30b8594df/texttracks?api-version=1.4-private";
            //using (HttpClient client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(apiUrl);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //    client.DefaultRequestHeaders.Add("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImtnMkxZczJUMENUaklmajRydDZKSXluZW4zOCIsImtpZCI6ImtnMkxZczJUMENUaklmajRydDZKSXluZW4zOCJ9.eyJhdWQiOiJodHRwczovLyoubWljcm9zb2Z0c3RyZWFtLmNvbSIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzFlZGFhZDgzLWIyZWYtNDgzZC04MWYxLTJjNDg2ODJmNDBlYy8iLCJpYXQiOjE2MDY0NjEwNDYsIm5iZiI6MTYwNjQ2MTA0NiwiZXhwIjoxNjA2NDY0OTQ2LCJhY3IiOiIxIiwiYWlvIjoiQVNRQTIvOFJBQUFBQ0w4RHZFSW9PM2JYOWU2c3lRRDlhSStmd0VqTWU2YlpyWnBlMGdrWmxhST0iLCJhbXIiOlsicHdkIl0sImFwcGlkIjoiY2Y1M2ZjZTgtZGVmNi00YWViLThkMzAtYjE1OGU3YjFjZjgzIiwiYXBwaWRhY3IiOiIyIiwiZmFtaWx5X25hbWUiOiJEaGFpZ3VkZSIsImdpdmVuX25hbWUiOiJBaml0IiwiaXBhZGRyIjoiMTUyLjU3LjE5OC4yMzYiLCJuYW1lIjoiRGhhaWd1ZGUsIEFqaXQgKENhcGl0YSBTb2Z0d2FyZSkiLCJvaWQiOiI1OWU5OGEyNC1jZGZhLTQxN2ItOWU3My0xZGQ5M2QzYjI0NmMiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMjM4NTc0OTg3LTI5MzUzODY4MTktMjA5MzY4NjEwLTI1NTc1NjIiLCJwdWlkIjoiMTAwMzIwMDA0M0QxMTJBMCIsInJoIjoiMC5BQUFBZzYzYUh1LXlQVWlCOFN4SWFDOUE3T2o4VThfMjN1dEtqVEN4V09leHo0TUNBTG8uIiwic2NwIjoiYWNjZXNzX21pY3Jvc29mdHN0cmVhbV9zZXJ2aWNlIiwic3ViIjoib3NOZUlNaEM2Zm13czNXakMxRTZUTmtfbHE2OVFwQTZSS2xNV3BqOEZlZyIsInRpZCI6IjFlZGFhZDgzLWIyZWYtNDgzZC04MWYxLTJjNDg2ODJmNDBlYyIsInVuaXF1ZV9uYW1lIjoiUDEwNDk0NzY1QGNhcGl0YS5jby51ayIsInVwbiI6IlAxMDQ5NDc2NUBjYXBpdGEuY28udWsiLCJ1dGkiOiJwcmRlWWZESVgwU3p4UnlKU25ZNEFRIiwidmVyIjoiMS4wIn0.NwdWzdPZ5rcBhQc08mlTChxQ3HCMcY9L4MZJSZAwprHjjhKjRNvam1366DD8aKpmKrfCZyyYAH7LDtw8FAEcK9qVqjw9i288Lwhpw2KvKZJ_NceUpUP2uLagjGht3ecE77tOk3HVJBZbPXfH-Wh36l9O1ZnH6VyqMV13ENwDrBN-g20JAf4oY8mPH7OtXcmh89pl7jttz1xXUS2AqCMo_pFULTA7X8LDQ4XGBcBYTM7oKLFwyHJ0rY4JlSpLPbS70Me55DBLU7LNcQyZfID5Bc8d02O340PYHYHoJ_Wo9tqksGCCdfYZGPe4SNjezDkDp_lR7TxzXEmanuepujz-OQ");
            //    HttpResponseMessage response = await client.GetAsync(apiUrl);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var data = await response.Content.ReadAsStringAsync();
            //    }
            //}



            var parser = new SubtitlesParser.Classes.Parsers.SubParser();

            var allFiles = BrowseTestSubtitlesFiles();
            foreach (var file in allFiles)
            {
                var fileName = Path.GetFileName(file);
                using (var fileStream = new FileStream(@file, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        var mostLikelyFormat = parser.GetMostLikelyFormat(fileName);
                        var items = parser.ParseStream(fileStream, Encoding.UTF8, mostLikelyFormat);
                        if (items.Any())
                        {
                            Console.WriteLine("Parsing of file {0}: SUCCESS ({1} items - {2}% corrupted)",
                                fileName, items.Count, (items.Count(it => it.StartTime <= 0 || it.EndTime <= 0) * 100) / items.Count);
                            /*foreach (var item in items)
                            {
                                Console.WriteLine(item);
                            }*/
                            /*var duplicates =
                                items.GroupBy(it => new {it.StartTime, it.EndTime}).Where(grp => grp.Count() > 1);
                            Console.WriteLine("{0} duplicate items", duplicates.Count());*/

                        }
                        else
                        {
                            //Console.WriteLine("Parsing of file {0}: SUCCESS (No items found!)", fileName, items.Count);
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
                Console.WriteLine("----------------------");
            }

            Console.ReadLine();


            return View();
        }

        private static string[] BrowseTestSubtitlesFiles()
        {
            const string subFilesDirectory = @"Content\TestFiles";
            var currentPath = Directory.GetCurrentDirectory();
            var completePath = Path.Combine(currentPath, subFilesDirectory);

            var allFiles = Directory.GetFiles(completePath);
            return allFiles;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
