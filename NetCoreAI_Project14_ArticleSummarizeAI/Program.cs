using Newtonsoft.Json;
using System.Text;

class Program
{
    private static readonly string apiKey = Environment.GetEnvironmentVariable("GROQ_API_PROJECT12");

    static async Task Main(string[] args)
    {
        Console.Write("Uzun metninizi veya makalenizi giriniz: ");
        string input;
        input = Console.ReadLine();

        if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine();
            Console.WriteLine("Girmiş olduğunuz metin AI tarafından özetleniyor...");
            Console.WriteLine();

            string shortSummary = "";
            string mediumSummary = "";
            string detailedSummary = "";

            shortSummary = await SummarizeArticle(input, "short");
            mediumSummary = await SummarizeArticle(input, "medium");
            detailedSummary = await SummarizeArticle(input, "detailed");

            Console.WriteLine("=== KISA ÖZET ===");
            Console.WriteLine(shortSummary);
            Console.WriteLine();
            Console.WriteLine("=== ORTA ÖZET ===");
            Console.WriteLine(mediumSummary);
            Console.WriteLine();
            Console.WriteLine("=== DETAYLI ÖZET ===");
            Console.WriteLine(detailedSummary);
        }
    }

    static async Task<string> SummarizeArticle(string text, string level)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            string prompt = level switch
            {
                "short" => "Summarize this text in Turkish in 1-2 sentences only.",
                "medium" => "Summarize this text in Turkish as 3-5 bullet points.",
                "detailed" => "Summarize this text in Turkish in detail with bullet points covering all key points.",
                _ => "Summarize this text in Turkish."
            };

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "You are an AI that summarizes articles in Turkish." },
                    new { role = "user", content = $"{prompt}\n\nText: {text}" }
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