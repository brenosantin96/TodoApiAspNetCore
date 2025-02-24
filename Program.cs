using ApiBreno01.Configurations;
using ApiBreno01.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Configurar o DbContext
builder.Services.AddDbContext<ConnectionContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

//A variável jwtConfig agora contém um objeto JwtConfig com os valores extraídos do appsettings.json, prontos para serem usados na configuração da autenticação JWT.
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
if (jwtConfig == null || string.IsNullOrEmpty(jwtConfig.Key))
{
    throw new InvalidOperationException("JWT configuration is missing or invalid.");
}

// Adicionar JwtConfig como um serviço 
//Essa linha é para criar injecao de dependencia.... Outras classes vao poder pegar o jwtConfig usando o seguinte: private readonly JwtConfig _jwtConfig;
builder.Services.AddSingleton(jwtConfig);


var key = Encoding.ASCII.GetBytes(jwtConfig.Key);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Definir como true em produção
    options.SaveToken = true; // Salvar o token no contexto da requisição. Define se o token deve ser salvo no contexto da requisição após a validação.

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Verifica a chave secreta //Define se a chave de assinatura do token deve ser validada, obrigatoria
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)), // Chave secreta usada para validar o token, obrigatoria

        ValidateIssuer = true, // Valida o emissor do token (Define se o emissor (issuer) do token deve ser validado) // middleware JWT Bearer verificará se a claim iss no token corresponde ao valor configurado em ValidIssuer. // o iss do token for diferente de https://localhost:7127, o token será rejeitado.
        ValidIssuer = jwtConfig.Issuer, // Emissor permitido (definido no appsettings.json)

        ValidateAudience = true, // Valida o público do token
        ValidAudience = jwtConfig.Audience, // Público permitido (definido no appsettings.json)

        ValidateLifetime = true, // Verifica se o token expirou, Define se o tempo de vida do token (validade) deve ser verificado. Isso garante que tokens expirados não sejam aceitos.
        ClockSkew = TimeSpan.Zero // Remove tempo extra para expiração do token Por padrão, o ClockSkew é de 5 minutos. Definir como TimeSpan.Zero remove essa tolerância.
    };
});

var app = builder.Build();

// Configurar o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar autenticação e autorização
app.UseAuthentication(); // Valida o token JWT
app.UseAuthorization();  // Controla o acesso com base nas permissões

app.MapControllers();
app.Run();


/*
JWT BEARER:
O Audience é a sua API, ou seja, o recurso que consumirá o token JWT para autorizar o acesso.
O Issuer é o servidor de autenticação que emite o token.
O Audience é o servidor de recursos (sua API) que valida e aceita o token.

Suponha que você esteja usando um serviço externo de autenticação, como o Auth0 ou Azure AD:
Issuer: O servidor de autenticação (por exemplo, https://meudominio.auth0.com/).
Audience: Sua API (por exemplo, https://api.meudominio.com).

ValidateIssuerSigningKey e IssuerSigningKey: Obrigatórios para validar a assinatura do token.
ValidateIssuer e ValidIssuer: Obrigatórios se você quiser validar o emissor do token.
ValidateAudience e ValidAudience: Obrigatórios se você quiser validar o público do token.
ValidateLifetime: Recomendado para garantir que tokens expirados não sejam aceitos.
As outras propriedades (ClockSkew, RequireHttpsMetadata, SaveToken) são opcionais e dependem das necessidades do seu projeto.
*/