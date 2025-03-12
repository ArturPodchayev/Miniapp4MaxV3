using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MiniApp4Max.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private static string _accessToken = "";  // ✅ Кешируем токен
        private static DateTime _tokenExpiration = DateTime.UtcNow;  // ✅ Время истечения токена

        public class CustomerRequest
        {
            public string apiLogin { get; set; }
            public string phone { get; set; }
            public string type { get; set; } = "phone";
            public string organizationId { get; set; }
        }

        private async Task<bool> RefreshTokenIfNeeded(string apiLogin)
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration.AddMinutes(-5))
            {
                Console.WriteLine("✅ Токен ещё активен, используем текущий.");
                return true;
            }

            Console.WriteLine("📡 Запрашиваем новый токен...");
            var tokenRequest = new { apiLogin = apiLogin };
            var response = await _httpClient.PostAsJsonAsync("https://api-ru.iiko.services/api/1/access_token", tokenRequest);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Ошибка получения токена: {response.StatusCode}");
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            dynamic tokenData = JsonConvert.DeserializeObject(content);

            _accessToken = tokenData.token;
            _tokenExpiration = DateTime.UtcNow.AddMinutes(55);  // ✅ Сохраняем токен на 55 минут

            Console.WriteLine("✅ Новый токен успешно получен!");
            return true;
        }

        [HttpPost("getCustomerInfo")]
        public async Task<IActionResult> GetCustomerInfo([FromBody] CustomerRequest request)
        {
            Console.WriteLine($"🔵 Запрос получен: phone={request.phone}, organizationId={request.organizationId}");

            if (!await RefreshTokenIfNeeded(request.apiLogin))
            {
                return StatusCode(500, "Ошибка обновления токена");
            }

            var customerRequest = new
            {
                organizationId = request.organizationId,
                phone = request.phone,
                type = "phone",
                extraData = true
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.PostAsJsonAsync(
                "https://api-ru.iiko.services/api/1/loyalty/iiko/customer/info",
                customerRequest
            );

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Ошибка запроса к API iiko: {response.StatusCode} - {error}");
                return BadRequest($"Ошибка запроса к API iiko: {response.StatusCode} - {error}");
            }

            string customerJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"🎉 Успешный ответ от iiko: {customerJson}");

            return Content(customerJson, "application/json");
        }
    }
}
