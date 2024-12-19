using Microsoft.AspNetCore.Mvc;
using VIS_projekt.Gateways;

namespace VIS_projekt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserGateway _userGateway;

        public UserController(UserGateway userGateway)
        {
            _userGateway = userGateway;
        }

        [HttpGet("trainer/{trainerId}/users")]
        public IActionResult GetUsersForTrainer(int trainerId)
        {
            try
            {
                var users = _userGateway.GetUsersByTrainerId(trainerId);

                if (users == null || users.Count == 0)
                {
                    return NotFound(new { message = "No users found for the specified trainer." });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error fetching users for trainer: {ex.Message}" });
            }
        }

        [HttpGet("unassigned")]
        public IActionResult GetUsersWithoutTrainer()
        {
            var users = _userGateway.GetUsersWithoutTrainer();
            return Ok(users);
        }

        [HttpPut("assignTrainer")]
        public IActionResult AssignTrainerToUser([FromQuery] int userId, [FromQuery] int trainerId)
        {
            _userGateway.AssignTrainerToUser(userId, trainerId);
            return Ok();
        }

        [HttpGet("{userId}/trainer")]
        public IActionResult GetUserTrainer(int userId)
        {
            var trainer = _userGateway.GetTrainerForUser(userId);
            if (trainer == null)
            {
                return NotFound(new { message = "No trainer assigned to this user." });
            }

            return Ok(trainer);
        }
    }
}
