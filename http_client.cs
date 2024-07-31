using Newtonsoft.Json;
using System.Net;

namespace TestClient
{
    class Entry
    {
        public string name { get; set; }
        public int score { get; set; }
        public int position { get; set; }
    }
    class Program
    {
        const string address = "http://localhost:3000";
        static async Task Main()
        {
            Random rand = new Random();
            const string alpha = "qwertyuioplkjhgfdsazxcvbnm";
            while (true)
            {
                Console.Write(" > ");
                var line = Console.ReadLine();

                if (line.StartsWith("post"))
                {
                    var name = line.Split(' ').Skip(1).First();
                    _ = int.TryParse(line.Split(' ').Skip(2).First(), out int score);

                    var resp = await SendHighscore(name, score);
                    var intermediate = JsonConvert.DeserializeObject<List<string>>(resp);
                    var body = intermediate.Select(i => JsonConvert.DeserializeObject<Entry>(i));
                    body?.OrderBy(x => x.position)?.ToList()?.ForEach(x => Console.WriteLine($"{x.position} : {x.name} : {x.score}"));
                }
                else if (line.StartsWith("get"))
                {
                    var resp = await GetHighscore();
                    var intermediate = JsonConvert.DeserializeObject<List<string>>(resp);
                    var body = intermediate.Select(i => JsonConvert.DeserializeObject<Entry>(i));
                    body?.OrderBy(x => x.position)?.ToList()?.ForEach(x => Console.WriteLine($"{x.position} : {x.name} : {x.score}"));
                }

            }
        }

        static async Task<string> GetHighscore()
        {
            using var client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(address);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        static async Task<string> SendHighscore(string name, int score)
        {
            var dict = new Dictionary<string, string>()
            {
                [nameof(name)] = name,
                [nameof(score)] = score.ToString()
            };
            var entry = new Entry { name = name, score = score, position = -1 };
            var content_str = JsonConvert.SerializeObject(entry);
            var content = new StringContent(content_str);

            using var client = new HttpClient();
            var hostName = Dns.GetHostName();
            var addresses = Dns.GetHostEntry(hostName);
            var myIP = addresses.AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            using HttpResponseMessage response = await client.PostAsync(address, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
