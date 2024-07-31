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
        static async Task Main()
        {
            while (true)
            {
                Console.Write(" name : ");
                var name = Console.ReadLine();

                Console.Write(" score : ");
                var scoreStr = Console.ReadLine();

                _ = int.TryParse(scoreStr, out var score);

                Console.WriteLine("");
                var resp = await SendHighscore(name, score);
                var intermediate = JsonConvert.DeserializeObject<List<string>>(resp);
                var body = intermediate.Select(i => JsonConvert.DeserializeObject<Entry>(i));
                body?.OrderBy(x => x.position)?.ToList()?.ForEach(x => Console.WriteLine($"{x.position} : {x.name} : {x.score}"));
                Console.WriteLine("");
            }
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

            using HttpResponseMessage response = await client.PostAsync($"http://{myIP}:8080/", content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
