using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Resim Yolunu Giriniz: ");
        string imagePath = Console.ReadLine();

        string apiKey = "Key'inizi Yazın"; 

        byte[] imageBytes = File.ReadAllBytes(imagePath);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var content = new ByteArrayContent(imageBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        var response = await httpClient.PostAsync(
            "https://router.huggingface.co/hf-inference/models/microsoft/trocr-large-printed",
            content);

        string responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<JsonElement>(responseString);
            Console.WriteLine("Resimdeki Metin:");
            Console.WriteLine(result[0].GetProperty("generated_text").GetString());
        }
        else
        {
            Console.WriteLine($"Hata: {responseString}");
        }
    }
}