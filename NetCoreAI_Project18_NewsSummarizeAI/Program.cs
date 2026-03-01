using Newtonsoft.Json;
using System.Text;
using System.Xml;

class Program
{
    private static readonly string apiKey = Environment.GetEnvironmentVariable("GROQ_API_PROJECT12");

    static async Task Main(string[] args)
    {
        Console.Write("RSS URL'sini giriniz: ");
        string rssUrl = Console.ReadLine();

        if (string.IsNullOrEmpty(rssUrl))
        {
            Console.WriteLine("URL boş olamaz!");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Haberler yükleniyor...");
        var news = await FetchRSS(rssUrl);

        if (news.Count == 0)
        {
            Console.WriteLine("Haber bulunamadı!");
            return;
        }

        
        int count = Math.Min(3, news.Count);
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {i + 1}. HABER ÖZETLENİYOR ---");
            string summary = await SummarizeNews(news[i].Title, news[i].Description);
            Console.WriteLine(summary);
            Console.WriteLine();
        }
    }

    static async Task<List<NewsItem>> FetchRSS(string rssUrl)
    {
        var newsList = new List<NewsItem>();

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                string xml = await client.GetStringAsync(rssUrl);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNodeList items = doc.SelectNodes("//item");

                foreach (XmlNode item in items)
                {
                    string title = item.SelectSingleNode("title")?.InnerText ?? "";
                    string description = item.SelectSingleNode("description")?.InnerText ?? "";
                    string link = item.SelectSingleNode("link")?.InnerText ?? "";

                    description = System.Text.RegularExpressions.Regex.Replace(description, "<.*?>", "");

                    newsList.Add(new NewsItem
                    {
                        Title = title,
                        Description = description,
                        Link = link
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("RSS çekme hatası: " + ex.Message);
        }

        return newsList;
    }

    static async Task<string> SummarizeNews(string title, string description)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Sen bir haber editörüsün. Verilen haberi Türkçe olarak kısaca özetle. Sadece özeti yaz, başlık yazma." },
                    new { role = "user", content = $"Başlık: {title}\n\nİçerik: {description}\n\nBu haberi özetle." }
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


class NewsItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }
}
