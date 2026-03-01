

using System.Text;
using System.Text.Json;

class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
        Console.WriteLine("Lütfen Sorunuzu Buraya Yazınız! (örnek: 'Bugün İstanbulda hava kaç derece?')");
        var prompt = Console.ReadLine();
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var requestBody = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new {role ="system", content ="You are a helpful assistant"},
                new {role ="user", content = prompt}
            },
            max_tokens = 2000
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseString);
                var answer = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                Console.WriteLine("AI'nın Cevabı:");
                Console.WriteLine(answer);
            }
            else
            {
                Console.WriteLine($"Bir hata oluştu: {response.StatusCode}");
                Console.WriteLine(responseString);
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        }

    }

}
