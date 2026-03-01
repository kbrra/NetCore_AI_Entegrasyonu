using System.Net.Http.Headers;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "Key'inizi Yazın";
        string audioFilePaht = "audio.1.mp3";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var form = new MultipartFormDataContent();

            var audioContent = new ByteArrayContent(File.ReadAllBytes(audioFilePaht));
            audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg");
            form.Add(audioContent, "file", Path.GetFileName(audioFilePaht));
            form.Add(new StringContent("whisper-large-v3"), "model");

            Console.WriteLine("Ses Dosyası İşleniyor, Lütfen Bekleyiniz....");

            var response = await client.PostAsync("https://api.groq.com/openai/v1/audio/transcriptions", form);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Transkript: ");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"Hata: {response.StatusCode}");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }

        }
    }
}