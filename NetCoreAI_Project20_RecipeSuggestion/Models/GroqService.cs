using Newtonsoft.Json;

using System.Text;

namespace NetCoreAI_Project20_RecipeSuggestion.Models

{

    public class GroqService

    {

        private readonly string _apiKey;

        private readonly HttpClient _httpClient;

        private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";

        private const string Model = "llama-3.3-70b-versatile";

        public GroqService()

        {

            _apiKey = Environment.GetEnvironmentVariable("GROQ_API_PROJECT12") ?? "";

            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        }

        public async Task<string> GetRecipeAsync(string ingredients)

        {

            var requestBody = BuildRequestBody(ingredients);

            var response = await SendRequestAsync(requestBody);

            return ParseResponse(response);

        }

        private object BuildRequestBody(string ingredients)

        {

            return new

            {

                model = Model,

                messages = new[]

                {

                    new { role = "system", content = "Sen bir aşçısın. Kullanıcının verdiği malzemelere göre Türkçe yemek tarifi öneriyorsun. Tarifi şu formatta ver: Yemek Adı, Malzemeler, Hazırlanışı adım adım." },

                    new { role = "user", content = $"Elimde şu malzemeler var: {ingredients}. Bana bir yemek tarifi öner." }

                }

            };

        }

        private async Task<string> SendRequestAsync(object requestBody)

        {

            string json = JsonConvert.SerializeObject(requestBody);

            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(ApiUrl, content);

            string responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)

                throw new Exception("Groq API hatası: " + responseJson);

            return responseJson;

        }

        private string ParseResponse(string responseJson)

        {

            var result = JsonConvert.DeserializeObject<dynamic>(responseJson);

            return result.choices[0].message.content.ToString();

        }

    }

}