using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ControlHub.Api.Services;

public class AuthService
{
    private readonly ControlHubDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(
        ControlHubDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email);

        if (usuario is null)
            return null;

        if (!usuario.Ativo)
            return null;

        var senhaValida = BCrypt.Net.BCrypt.Verify(
            dto.Senha,
            usuario.SenhaHash);

        if (!senhaValida)
            return null;

        var token = GerarToken(usuario);

        return new LoginResponseDto
        {
            Token = token,
            ExpiraEm = DateTime.UtcNow.AddHours(8),
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil.ToString(),
            EmpresaId = usuario.EmpresaId
        };
    }

    private string GerarToken(Models.Usuario usuario)
    {
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        if (string.IsNullOrWhiteSpace(jwtSecret))
            throw new InvalidOperationException(
                "JWT_SECRET não foi encontrado.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        if (usuario.EmpresaId.HasValue)
        {
            claims.Add(
                new Claim(
                    "EmpresaId",
                    usuario.EmpresaId.Value.ToString()));
        }
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}