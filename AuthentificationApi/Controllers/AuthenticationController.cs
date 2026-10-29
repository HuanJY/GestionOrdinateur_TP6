using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{

    private readonly IConfiguration _configuration;


    private readonly IHttpClientFactory _httpClientFactory;


    private readonly KeycloakAuthService _keycloakAuthService;

   
    public AuthenticationController(IConfiguration configuration, IHttpClientFactory httpClientFactory, KeycloakAuthService keycloakAuthService)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _keycloakAuthService = keycloakAuthService;
    }

  
    [HttpPost("login")]
    public async Task<IActionResult> Login(string login)
    {
        var client = _httpClientFactory.CreateClient("GestionOrdinateursApi");
        var reponse = await client.GetAsync($"users/{login}");

        if (!reponse.IsSuccessStatusCode)
            return NotFound("L'utilisateur n'existe pas");

        var user = await reponse.Content.ReadFromJsonAsync<User>();
        if (user == null)
            return NotFound("L'utilisateur n'existe pas");

        var jwtKey = _configuration["Jwt:Key"] ?? "";
        var key = Encoding.ASCII.GetBytes(jwtKey);

        return Ok(GenTokenJWT(key, user.Role, user.Login));
    }

   
    [HttpPost("login-keycloak")]
    public async Task<IActionResult> LoginKeycloak(KeycloakLoginRequest request)
    {
        try
        {
            var token = await _keycloakAuthService.LoginAsync(request.Username, request.Password);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    
    private string GenTokenJWT(byte[] key, RoleEnum role, string login)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}