using Newtonsoft.Json;
using System.Text;

class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Çevirmek İstediğimiz Cümleyi Giriniz: ");
        string inputText = Console.ReadLine();

        string translatedText = await TransladeTextToEnglish(inputText);

        if (!string.IsNullOrEmpty(translatedText))
        {
            Console.WriteLine($"İngilizce çeviri: {translatedText}");
        }
        else
        {
            Console.WriteLine("Beklenmeyen bir hata oluştu");
        }
    }

    private static async Task<string> TransladeTextToEnglish(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(
                    $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair=tr|en");

                string responseString = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseString);
                string translation = responseObject.responseData.translatedText;
                return translation;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                return null;
            }
        }
    }
}