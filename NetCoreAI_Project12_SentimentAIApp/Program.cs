using Newtonsoft.Json;
using System.Text;

class Program
{
    private static string apiKey = Environment.GetEnvironmentVariable("GROQ_API_PROJECT12");

    static async Task Main(string[] args)
    {
        Console.WriteLine("Lütfen Metni Giriniz:");
        string input = Console.ReadLine();

        if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine();
            Console.WriteLine("Duygu analizi yapılıyor...");
            Console.WriteLine();
            string sentiment = await AnalyzeSentiment(input);
            Console.WriteLine($"Sonuç: {sentiment}");
        }
    }

    static async Task<string> AnalyzeSentiment(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                 {
                    new { role = "system", content = "You are an AI that analyzes sentiment. You categorize text as Positive, Negative or Neutral" },
                    new { role = "user", content = $"Analyze the sentiment of this text: \"{text}\" and return only Positive, Negative or Neutral" }
                }
            };
            string json = JsonConvert.SerializeObject(requestBody);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                string analyzsis = result.choices[0].message.content.ToString();
                return analyzsis;
            }
            else
            {
                Console.WriteLine("Bir hata oluştu: " + responseJson);
                return "Hata";
            }
        }
    }
}