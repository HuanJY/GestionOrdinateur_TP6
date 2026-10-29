using System.Text.Json;
using GestionOrdinateurs.Shared;

namespace AuthentificationApi;

public class KeycloakAuthService
{

    private readonly HttpClient _httpClient;


    private readonly IConfiguration _configuration;

    public KeycloakAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }


    public async Task<TokenResponse> LoginAsync(string username, string password)
    {
        var tokenEndpoint = $"{_configuration["Keycloak:BaseUrl"]}/realms/{_configuration["Keycloak:Realm"]}/protocol/openid-connect/token";

        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", _configuration["Keycloak:ClientId"]! },
            { "client_secret", _configuration["Keycloak:ClientSecret"]! },
            { "username", username },
            { "password", password }
        };

        var content = new FormUrlEncodedContent(requestBody);
        var response = await _httpClient.PostAsync(tokenEndpoint, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResult = JsonSerializer.Deserialize<TokenResponse>(responseContent);
            return tokenResult!;
        }

        //throw new Exception("Identifiant ou mot de passe incorrect");
        // Erreur renvoyée par Keycloak (affichée pour comprendre la cause du refus)
            var erreur = await response.Content.ReadAsStringAsync();
            throw new Exception($"Keycloak a refusé la connexion ({(int)response.StatusCode}) : {erreur}");
    }
}