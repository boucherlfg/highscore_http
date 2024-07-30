using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace TestClient 
{
	class Program 
	{
		static void Main() 
		{
			while (true)
			{
				try
				{
					Console.Write(" name : ");
					var name = Console.ReadLine();

					Console.Write(" score : ");
					var scoreStr = Console.ReadLine();

					_ = int.TryParse(scoreStr, out var score);

					Console.WriteLine("");
				var resp = await SendHighscore(name, score);
					Dictionary<string, string>? body = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
				body?.ToList()?.ForEach(x => Console.WriteLine($"{x.Key} : {x.Value}"));
					Console.WriteLine("");
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		async Task<string> SendHighscore(string name, int score)
		{
			var dict = new Dictionary<string, string>()
			{
				[nameof(name)] = name,
				[nameof(score)] = score.ToString()
			};
			var content = new FormUrlEncodedContent(dict);

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
