using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace PLD_generator
{
    class Program
    {
        public class CFields {
            public string Name { get; set; }
            public string TaskType { get; set; }
        }

        public class Record
        {
            public string Id { get; set; }
            public CFields Fields { get; set; }
        }

        static public async Task<dynamic> GetRecords()
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                $"Bearer {Configuration.AirtableApiKey}");

            try
            {
                HttpResponseMessage response = await client.GetAsync(
                    "https://api.airtable.com/v0/appmxiObOjwHE5onb/PLD?view=Kanban%20Assignation");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseBody2 = JObject.Parse(responseBody);

                return new {
                    ok = true,
                    msg = responseBody2
                };
            }
            catch (HttpRequestException e)
            {
                return new {
                    ok = false,
                    error = e.Message
                };
            }
        }

        static public List<List<Record>> GetTasks(JArray records)
        {
            List<dynamic> records2 = records.ToObject<List<dynamic>>();
            var records3 = records2
                .Select(task => new Record {
                    Id = task.id,
                    Fields = new CFields {
                        Name = task.fields.Name,
                        TaskType = task.fields["Task Type"]
                    }
                })
                .GroupBy(task => task.Fields.TaskType)
                .Select(taskGroup => taskGroup.ToList())
                .ToList();

            return records3;
        }

        static async Task Main(string[] args)
        {
            Configuration.Load();

            var res = await GetRecords();

            List<List<Record>> tasks = GetTasks(res.msg.records);

            tasks.ForEach(taskGroup => {
                Console.WriteLine(taskGroup[0].Fields.TaskType);
                taskGroup.ForEach(
                    task => Console.WriteLine("\t" + task.Fields.Name));
            });
        }
    }
}
