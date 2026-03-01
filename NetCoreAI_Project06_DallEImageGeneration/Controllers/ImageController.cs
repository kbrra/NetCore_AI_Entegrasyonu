using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

public class ImageController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string prompt)
    {
        string apiKey = "Key'inizi Yazın"; 

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new { inputs = prompt };
            string jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(
                "https://router.huggingface.co/hf-inference/models/stabilityai/stable-diffusion-xl-base-1.0",
                content);

            if (!response.IsSuccessStatusCode)
            {
                string err = await response.Content.ReadAsStringAsync();
                ViewBag.Error = err;
                return View();
            }

            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            string base64 = Convert.ToBase64String(imageBytes);
            ViewBag.ImageUrl = $"data:image/jpeg;base64,{base64}";
            ViewBag.Prompt = prompt;
        }

        return View();
    }
}