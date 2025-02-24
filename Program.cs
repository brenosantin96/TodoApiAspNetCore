using ApiBreno01.Configurations;
using ApiBreno01.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicionar servi�os ao cont�iner
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Configurar o DbContext
builder.Services.AddDbContext<ConnectionContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

//A vari�vel jwtConfig agora cont�m um objeto JwtConfig com os valores extra�dos do appsettings.json, prontos para serem usados na configura��o da autentica��o JWT.
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
if (jwtConfig == null || string.IsNullOrEmpty(jwtConfig.Key))
{
    throw new InvalidOperationException("JWT configuration is missing or invalid.");
}

// Adicionar JwtConfig como um servi�o 
//Essa linha � para criar injecao de dependencia.... Outras classes vao poder pegar o jwtConfig usando o seguinte: private readonly JwtConfig _jwtConfig;
builder.Services.AddSingleton(jwtConfig);


var key = Encoding.ASCII.GetBytes(jwtConfig.Key);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Definir como true em produ��o
    options.SaveToken = true; // Salvar o token no contexto da requisi��o. Define se o token deve ser salvo no contexto da requisi��o ap�s a valida��o.

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Verifica a chave secreta //Define se a chave de assinatura do token deve ser validada, obrigatoria
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)), // Chave secreta usada para validar o token, obrigatoria

        ValidateIssuer = true, // Valida o emissor do token (Define se o emissor (issuer) do token deve ser validado) // middleware JWT Bearer verificar� se a claim iss no token corresponde ao valor configurado em ValidIssuer. // o iss do token for diferente de https://localhost:7127, o token ser� rejeitado.
        ValidIssuer = jwtConfig.Issuer, // Emissor permitido (definido no appsettings.json)

        ValidateAudience = true, // Valida o p�blico do token
        ValidAudience = jwtConfig.Audience, // P�blico permitido (definido no appsettings.json)

        ValidateLifetime = true, // Verifica se o token expirou, Define se o tempo de vida do token (validade) deve ser verificado. Isso garante que tokens expirados n�o sejam aceitos.
        ClockSkew = TimeSpan.Zero // Remove tempo extra para expira��o do token Por padr�o, o ClockSkew � de 5 minutos. Definir como TimeSpan.Zero remove essa toler�ncia.
    };
});

var app = builder.Build();

// Configurar o pipeline de requisi��es HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar autentica��o e autoriza��o
app.UseAuthentication(); // Valida o token JWT
app.UseAuthorization();  // Controla o acesso com base nas permiss�es

app.MapControllers();
app.Run();


/*
JWT BEARER:
O Audience � a sua API, ou seja, o recurso que consumir� o token JWT para autorizar o acesso.
O Issuer � o servidor de autentica��o que emite o token.
O Audience � o servidor de recursos (sua API) que valida e aceita o token.

Suponha que voc� esteja usando um servi�o externo de autentica��o, como o Auth0 ou Azure AD:
Issuer: O servidor de autentica��o (por exemplo, https://meudominio.auth0.com/).
Audience: Sua API (por exemplo, https://api.meudominio.com).

ValidateIssuerSigningKey e IssuerSigningKey: Obrigat�rios para validar a assinatura do token.
ValidateIssuer e ValidIssuer: Obrigat�rios se voc� quiser validar o emissor do token.
ValidateAudience e ValidAudience: Obrigat�rios se voc� quiser validar o p�blico do token.
ValidateLifetime: Recomendado para garantir que tokens expirados n�o sejam aceitos.
As outras propriedades (ClockSkew, RequireHttpsMetadata, SaveToken) s�o opcionais e dependem das necessidades do seu projeto.
*/