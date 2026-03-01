using Newtonsoft.Json;
using System.Text;
using UglyToad.PdfPig;

class Program
{
    private static readonly string apiKey = Environment.GetEnvironmentVariable("GROQ_API_PROJECT12");

    static async Task Main(string[] args)
    {
        Console.Write("PDF dosyasının yolunu giriniz: ");
        string pdfPath = Console.ReadLine();

        if (!File.Exists(pdfPath))
        {
            Console.WriteLine("Dosya bulunamadı!");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("PDF okunuyor...");
        string pdfContent = ExtractTextFromPdf(pdfPath);

        if (string.IsNullOrEmpty(pdfContent))
        {
            Console.WriteLine("PDF içeriği okunamadı!");
            return;
        }

        Console.WriteLine("PDF başarıyla okundu!");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("Ne yapmak istersiniz?");
            Console.WriteLine("1 - PDF'i Özetle");
            Console.WriteLine("2 - PDF'e Soru Sor");
            Console.WriteLine("3 - Çıkış");
            Console.Write("Seçiminiz: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            if (choice == "1")
            {
                Console.WriteLine("PDF özetleniyor...");
                string summary = await SummarizePdf(pdfContent);
                Console.WriteLine();
                Console.WriteLine("=== ÖZET ===");
                Console.WriteLine(summary);
            }
            else if (choice == "2")
            {
                Console.Write("Sorunuzu giriniz: ");
                string question = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine("Cevap aranıyor...");
                string answer = await AskQuestion(pdfContent, question);
                Console.WriteLine();
                Console.WriteLine("=== CEVAP ===");
                Console.WriteLine(answer);
            }
            else if (choice == "3")
            {
                break;
            }

            Console.WriteLine();
        }
    }

    static string ExtractTextFromPdf(string pdfPath)
    {
        try
        {
            StringBuilder text = new StringBuilder();
            using (var pdf = PdfDocument.Open(pdfPath))
            {
                foreach (var page in pdf.GetPages())
                {
                    text.AppendLine(page.Text);
                }
            }
            return text.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine("PDF okuma hatası: " + ex.Message);
            return "";
        }
    }

    static async Task<string> SummarizePdf(string content)
    {
        
        if (content.Length > 5000)
            content = content.Substring(0, 5000);

        return await SendToGroq(
            "Sen bir PDF analiz uzmanısın. Verilen PDF içeriğini Türkçe olarak madde madde özetle.",
            $"Bu PDF'i özetle:\n\n{content}"
        );
    }

    static async Task<string> AskQuestion(string content, string question)
    {
        if (content.Length > 5000)
            content = content.Substring(0, 5000);

        return await SendToGroq(
            "Sen bir PDF analiz uzmanısın. Verilen PDF içeriğine göre soruları Türkçe olarak cevapla. Eğer cevap PDF'de yoksa 'Bu bilgi PDF'de bulunmuyor' de.",
            $"PDF içeriği:\n\n{content}\n\nSoru: {question}"
        );
    }

    static async Task<string> SendToGroq(string systemMessage, string userMessage)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = systemMessage },
                    new { role = "user", content = userMessage }
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
