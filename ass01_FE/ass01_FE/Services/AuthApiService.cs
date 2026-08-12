namespace ass01_FE.Services;

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ass01_FE.Models.Auth;

public class AuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new System.Uri(configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7087/");
    }

    public async Task<LoginResponse?> LoginAsync(LoginViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        return null;
    }
}
