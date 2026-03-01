using System.Net.Http.Headers;

class Program

{

    private static string apiKey = "Key'inizi Yazın";

    private static string voiceId = "21m00Tcm4TlvDq8ikWAM";

    static async Task Main(string[] argms)

    {

        Console.WriteLine("Metni Giriniz:");

        string input;

        input = Console.ReadLine();

        if (!string.IsNullOrEmpty(input))

        {

            Console.WriteLine("Ses dosyası oluşturuluyor....");

            bool basarili = await GenerateSpeech(input);

            if (basarili)

            {

                Console.WriteLine("Ses dosyası 'output.mp3' olarak kaydedildi.");

                System.Diagnostics.Process.Start("explorer.exe", "output.mp3");

            }

        }

    }

    static async Task<bool> GenerateSpeech(string text)

    {

        using (HttpClient client = new HttpClient())

        {

            client.DefaultRequestHeaders.Add("xi-api-key", apiKey);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/mpeg"));

            var requestBody = new

            {

                text = text,

                model_id = "eleven_multilingual_v2",

                voice_settings = new

                {

                    stability = 0.5,

                    similarity_boost = 0.75

                }

            };

            string json = System.Text.Json.JsonSerializer.Serialize(requestBody);

            HttpContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(

                $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}", content);

            if (response.IsSuccessStatusCode)

            {

                byte[] audioBytes = await response.Content.ReadAsByteArrayAsync();

                await File.WriteAllBytesAsync("output.mp3", audioBytes);

                return true;

            }

            else

            {

                string hata = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Bir hata oluştu: " + hata);

                return false;

            }

        }

    }

}