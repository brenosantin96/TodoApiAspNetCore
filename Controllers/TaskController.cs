using ApiBreno01.Context;
using ApiBreno01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Task = ApiBreno01.Models.Task;

namespace ApiBreno01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {

        private readonly ConnectionContext _context;

        public TaskController(ConnectionContext context)
        {
            _context = context;
        }


        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { msg = "pong", msg2 = "ping" });
        }

        [HttpGet("All")]
        public IActionResult GetAll()
        {
            var tasks = _context.Tasks.ToList();

            if (!tasks.Any())
            {
                return NotFound(new { message = "No tasks found" });
            }

            return Ok(tasks);

        }

        [HttpGet("user/{userId}/tasks")]
        public IActionResult GetTasksByUser(int userId)
        {
            var user = _context.Users.Include(u => u.Tasks)
                .FirstOrDefault(u => u.Id == userId);


            if (user == null) {
                return BadRequest(new { message = "User not found." });
            }

            return Ok(user.Tasks);
        }

        [HttpGet("GetTaskById/{id}")]
        public IActionResult getById(int id)
        {

            var task = _context.Tasks.FirstOrDefault((task) => task.Id == id);
            if (task == null)
            {
                return NotFound(new { message = $"Task with {id} not found" });
            }
            return Ok(task);
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] Task task)
        {

            
            if (task == null)
            {
                return BadRequest(new { message = "Task cannot be null." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // validation errors
            }

            // Verifica se o usuário existe
            var user = _context.Users.FirstOrDefault(u => u.Id == task.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            if (task.IsFinished == true && task.DateFinished == null)
            {
                return BadRequest(new {message = "Task cant be finished without a DateFinished"}); 
            }

            task.DateCreated = DateTime.Now;


            _context.Tasks.Add(task);
            _context.SaveChanges();

            return CreatedAtAction(nameof(getById), new { id = task.Id }, task);

        }

        [HttpPut("ChangeTask/{id}")]
        public IActionResult EditTask(int id, [FromBody] Task task)
        {
            if (id == 0)
            {
                return BadRequest(new { message = "Id needs to be informed." });
            }

            if (task == null)
            {
                return BadRequest(new { message = "Task data cannot be null." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna os erros de validação
            }

            var taskItem = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (taskItem == null)
            {
                return NotFound("Task not found");
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(task.Name))
                {
                    taskItem.Name = task.Name;
                }

                if (task.DateToFinish != null)
                {
                    taskItem.DateToFinish = task.DateToFinish;
                }

                if (task.IsFinished)
                {
                    taskItem.IsFinished = true;
                    if (taskItem.DateFinished == null)
                    {
                        taskItem.DateFinished = DateTime.Now; // Define a data de conclusão se não for fornecida
                    }
                }
                else
                {
                    taskItem.IsFinished = false;
                    taskItem.DateFinished = null; // Limpa a data de conclusão se a tarefa não estiver concluída
                }

                
                _context.SaveChanges();

                return Ok(new { message = "Task updated successfully.", task = taskItem });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task.", details = ex.Message });
            }

        }

        [HttpDelete ("Delete/{id}")]
        public IActionResult DeleteTask(int id) {

            var task = _context.Tasks.FirstOrDefault((t) => t.Id == id);
            if (task == null)
            {
                return NotFound(new { errorMessage = $"Task with {id} does not exists." });
            }

            var removedTask = task;
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return Ok(new {msg = "Task removed with success: ", removedTask});
        }


    }
}
