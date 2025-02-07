using ApiBreno01.Context;
using ApiBreno01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
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
                return BadRequest(ModelState); // Retorna os erros de validação
            }

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

                if (task.IsFinished != null) // Se IsFinished for um campo opcional
                {
                    taskItem.IsFinished = task.IsFinished;
                }

                _context.Tasks.Update(taskItem);
                _context.SaveChanges();
                return Ok(taskItem);
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
