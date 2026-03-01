using Newtonsoft.Json;
using System.Text;
using HtmlAgilityPack;

class Program
{
    private static readonly string apiKey = Environment.GetEnvironmentVariable("GROQ_API_PROJECT12");

    static async Task Main(string[] args)
    {
        Console.Write("Analiz edilecek URL'yi giriniz: ");
        string url = Console.ReadLine();

        if (!string.IsNullOrEmpty(url))
        {
            Console.WriteLine();
            Console.WriteLine("Sayfa içeriği çekiliyor...");
            string pageContent = await ScrapeWebsite(url);

            if (!string.IsNullOrEmpty(pageContent))
            {
                Console.WriteLine("İçerik analiz ediliyor...");
                Console.WriteLine();
                string analysis = await AnalyzeContent(pageContent);
                Console.WriteLine("=== SAYFA ANALİZİ ===");
                Console.WriteLine(analysis);
            }
        }
    }

    static async Task<string> ScrapeWebsite(string url)
    {
        try
        {
            var web = new HtmlWeb();
            var document = await Task.Run(() => web.Load(url));

          
            var scripts = document.DocumentNode.SelectNodes("//script|//style");
            if (scripts != null)
                foreach (var script in scripts)
                    script.Remove();

          
            string content = document.DocumentNode.InnerText;

            
            content = string.Join(" ", content.Split(new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries));

           
            if (content.Length > 3000)
                content = content.Substring(0, 3000);

            Console.WriteLine("İçerik başarıyla çekildi!");
            Console.WriteLine();
            return content;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Hata: " + ex.Message);
            return "";
        }
    }

    static async Task<string> AnalyzeContent(string content)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Sen bir web sitesi analiz uzmanısın. Verilen sayfa içeriğini Türkçe olarak analiz et ve şunları söyle: sitenin amacı, kategorisi, sunduğu hizmetler ve hedef kitlesi." },
                    new { role = "user", content = $"Bu web sitesinin içeriğini analiz et:\n\n{content}" }
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", httpContent);
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
