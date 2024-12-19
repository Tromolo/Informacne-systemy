using Microsoft.AspNetCore.Mvc;
using VIS_projekt.Controllers.Models;
using VIS_projekt.DTOs;
using VIS_projekt.Gateways;
using System;

namespace VIS_projekt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserGateway _userGateway;
        private readonly TrainerGateway _trainerGateway;

        public AuthController(UserGateway userGateway, TrainerGateway trainerGateway)
        {
            _userGateway = userGateway;
            _trainerGateway = trainerGateway;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (_userGateway.VerifyUserCredentials(loginRequest.Email, loginRequest.Password))
                {
                    var user = _userGateway.FindByEmail(loginRequest.Email);
                    return Ok(new { message = "Login successful", role = "user", id = user?.Id });
                }

                if (_trainerGateway.VerifyTrainerCredentials(loginRequest.Email, loginRequest.Password))
                {
                    var trainer = _trainerGateway.FindByEmail(loginRequest.Email);
                    return Ok(new { message = "Login successful", role = "trainer", id = trainer?.Id });
                }

                return Unauthorized(new { message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error during login: {ex.Message}" });
            }
        }

        [HttpPost("register/user")]
        public IActionResult RegisterUser([FromBody] RegisterUserRequest request)
        {
            try
            {
                var user = new User
                {
                    TrainerId = request.TrainerId == 0 ? null : request.TrainerId,
                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email,
                    Phone = request.Phone,
                    Role = request.Role
                };

                _userGateway.Register(user, request.Password);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error registering user: {ex.Message}" });
            }
        }

        [HttpPost("register/trainer")]
        public IActionResult RegisterTrainer([FromBody] RegisterTrainerRequest request)
        {
            try
            {
                var trainer = new Trainer
                {
                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email,
                    Phone = request.Phone,
                    Specialisation = request.Specialisation,
                    Skills = request.Skills
                };

                _trainerGateway.RegisterTrainer(trainer, request.Password);
                return Ok(new { message = "Trainer registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error registering trainer: {ex.Message}" });
            }
        }
    }
}
