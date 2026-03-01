using Newtonsoft.Json;
using System.Text;

class Program
{
    private static readonly string apiKey = Environment.GetEnvironmentVariable("GROQ_API_PROJECT12");

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== HİKAYE OLUŞTURUCU ===");
        Console.WriteLine();

        Console.Write("Karakter adı giriniz: ");
        string character = Console.ReadLine();

        Console.Write("Karakter türü giriniz (zaman yolcusu, siber hacker, gölge suikastçı, yapay zeka varlığı vs): ");
        string characterType = Console.ReadLine();

        Console.Write("Mekan giriniz (batık şehir, terk edilmiş uzay istasyonu, neon ışıklı mega kent, buzlarla kaplı krallık, paralel evren vs): ");
        string location = Console.ReadLine();

        Console.Write("Hikaye türü giriniz (macera, korku, komedi, romantik vs): ");
        string genre = Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine("Hikaye oluşturuluyor...");
        Console.WriteLine();

        string story = await GenerateStory(character, characterType, location, genre);
        Console.WriteLine("=== HİKAYENİZ ===");
        Console.WriteLine();
        Console.WriteLine(story);
    }

    static async Task<string> GenerateStory(string character, string characterType, string location, string genre)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Sen yaratıcı bir hikaye yazarısın. Verilen karakter, mekan ve türe göre Türkçe, akıcı ve sürükleyici kısa hikayeler yazarsın." },
                    new { role = "user", content = $"Bana şu bilgilere göre kısa bir hikaye yaz:\n\nKarakter adı: {character}\nKarakter türü: {characterType}\nMekan: {location}\nHikaye türü: {genre}\n\nHikaye en az 3 paragraf olsun." }
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions", content);

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
