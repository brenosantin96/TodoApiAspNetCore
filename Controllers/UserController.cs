using ApiBreno01.Context;
using ApiBreno01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ApiBreno01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ConnectionContext _context;

        public UserController(ConnectionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); //Se context for nulo, ele lança uma exceção ArgumentNullException
        }

        [HttpGet("all")]
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
        public IActionResult Create([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Usuário inválido.");
            }

            // Lógica para salvar o usuário...

            // Verifica se o modelo é válido, estou removendo toda a validacao de antes que tinha aqui
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna os erros de validação
            }

            _context.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        
    }
}

/*
 * IActionResult é uma interface, mas, nesse contexto, 
 * ela não é aplicada como uma herança (com :). Ela é o tipo de retorno do método, 
 * indicando que o método retornará algo que implementa a interface IActionResult.
 * */
