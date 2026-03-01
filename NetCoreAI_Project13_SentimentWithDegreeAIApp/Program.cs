using Newtonsoft.Json;
using System.Text;

class Program
{
    private static string apiKey = Environment.GetEnvironmentVariable("GROQ_API_PROJECT12");

    static async Task Main(string[] args)
    {
        Console.WriteLine("Lütfen Metni Giriniz:");
        string input = Console.ReadLine();
        Console.WriteLine();

        if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Gelişmiş Duygu analizi yapılıyor...");
            string sentiment = await AdvancedSentimentalAnalysis(input);
            Console.WriteLine();
            Console.WriteLine($"Sonuç: {sentiment}");
        }
    }

    static async Task<string> AdvancedSentimentalAnalysis(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "You are an advanced AI that analyzes emotions in text. Your response must be in JSON format. Identify the sentiment scores (0-100%) for the following emotions: Joy, Sadness, Anger, Fear, Surprise and Neutral." },
                    new { role = "user", content = $"Analyze this text: \"{text}\" and return a JSON object with percentages for each emotion." }
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                return result.choices[0].message.content.ToString();
            }
            else
            {
                Console.WriteLine("Bir hata oluştu: " + responseJson);
                return "Hata";
            }
        }
    }
}