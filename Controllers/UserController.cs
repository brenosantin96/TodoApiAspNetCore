using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiBreno01.Configurations;
using ApiBreno01.Context;
using ApiBreno01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;

namespace ApiBreno01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ConnectionContext _context;
        private readonly JwtConfig _jwtConfig;

        public UserController(ConnectionContext context, JwtConfig jwtConfig)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); //Se context for nulo, ele lança uma exceção ArgumentNullException
            _jwtConfig = jwtConfig ?? throw new ArgumentNullException(nameof(jwtConfig));
        }

        [HttpGet("All")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();

            if (users == null || !users.Any())
            {
                return NotFound(new { message = "No users found" });
            }

            return Ok(users);

        }

        [HttpGet("GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {

            var user = _context.Users.FirstOrDefault(user => user.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna os erros de validação
            }

            var emailExists = _context.Users.Any((u) => u.Email == user.Email);

            if (emailExists)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }

            _context.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {

            var userExists = _context.Users.FirstOrDefault((u) => u.Email == loginRequest.Email); //returns a user if it exists.
            if (userExists == null)
            {
                return BadRequest(new { message = "User with this email does not exist." });
            }

            if (userExists.Email == loginRequest.Email && userExists.Password == loginRequest.Password)
            {
                //Logado com sucesso, o que fazer agora?!?! 
                //Criar uma chave secreta a partir do nosso appsettings.json.
                //Definir as credenciais para assinar o token.
                //Criar as claims (dados adicionais no token, como userId e email).
                //Gerar e retornar o token

                // 1 -Pegando as configurações do JWT
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                //2 - Define as claims (informações do usuário dentro do token)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userExists.Id.ToString()),
                    new Claim(ClaimTypes.Email, userExists.Email)
                };

                // 3 -  Cria o token JWT
                var token = new JwtSecurityToken(
                    issuer: _jwtConfig.Issuer,
                    audience: _jwtConfig.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtConfig.TokenValidityMins),
                    signingCredentials: credentials
                );

                // 4️ - Retorna o token como resposta
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });

            }

            return BadRequest(new { message = "Email or password is incorrect." });

        }


    }
}

/*
 * IActionResult é uma interface, mas, nesse contexto, 
 * ela não é aplicada como uma herança (com :). Ela é o tipo de retorno do método, 
 * indicando que o método retornará algo que implementa a interface IActionResult.
 * */
