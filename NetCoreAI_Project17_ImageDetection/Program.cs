using Newtonsoft.Json;
using System.Text;

class Program
{
    private static readonly string apiKey = Environment.GetEnvironmentVariable("Project17");

    static async Task Main(string[] args)
    {
        Console.Write("Resim dosyasının yolunu giriniz: ");
        string imagePath = Console.ReadLine();

        if (!File.Exists(imagePath))
        {
            Console.WriteLine("Dosya bulunamadı!");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Resim analiz ediliyor...");
        Console.WriteLine();

        string result = await AnalyzeImage(imagePath);
        Console.WriteLine("=== RESİM ANALİZİ ===");
        Console.WriteLine(result);
    }

    static async Task<string> AnalyzeImage(string imagePath)
    {
        try
        {
            byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
            string base64Image = Convert.ToBase64String(imageBytes);

            string extension = Path.GetExtension(imagePath).ToLower();
            string mimeType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "openrouter/free",
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new
                                {
                                    type = "image_url",
                                    image_url = new
                                    {
                                        url = $"data:{mimeType};base64,{base64Image}"
                                    }
                                },
                                new
                                {
                                    type = "text",
                                    text = "Bu resmi Türkçe olarak detaylıca analiz et. Resimde ne var, ne oluyor?"
                                }
                            }
                        }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(
                    "https://openrouter.ai/api/v1/chat/completions", content);

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
        catch (Exception ex)
        {
            Console.WriteLine("Hata: " + ex.Message);
            return "Hata";
        }
    }
}