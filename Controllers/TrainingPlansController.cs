using Microsoft.AspNetCore.Mvc;
using VIS_projekt.Services;
using VIS_projekt.Controllers.Models;
using VIS_projekt.Domain;
using System;

namespace VIS_projekt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingPlansController : ControllerBase
    {
        private readonly TrainingPlanService _trainingPlanService;

        public TrainingPlansController(TrainingPlanService trainingPlanService)
        {
            _trainingPlanService = trainingPlanService;
        }


        [HttpPost]
        public IActionResult Create([FromBody] CreateTrainingPlanRequest request)
        {
            try
            {
                _trainingPlanService.AddTrainingPlan(request.UserId, request.Description ?? "", request.Active);
                return Ok(new { message = "Training plan created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error creating training plan: {ex.Message}" });
            }
        }


        [HttpGet("user/{userId}")]
        public IActionResult GetUserPlans(int userId)
        {
            try
            {
                var plans = _trainingPlanService.GetTrainingPlansByUserId(userId);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error fetching user plans: {ex.Message}" });
            }
        }

        [HttpGet("trainer/{trainerId}")]
        public IActionResult GetTrainerPlans(int trainerId)
        {
            try
            {
                var plans = _trainingPlanService.GetTrainingPlansByTrainerId(trainerId);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error fetching trainer plans: {ex.Message}" });
            }
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var plan = _trainingPlanService.GetTrainingPlanById(id);
                if (plan == null)
                {
                    return NotFound(new { message = "Training plan not found" });
                }
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error fetching training plan: {ex.Message}" });
            }
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateTrainingPlanRequest request)
        {
            try
            {
                var plan = _trainingPlanService.GetTrainingPlanById(id);
                if (plan == null)
                    return NotFound(new { message = "Training plan not found" });

                plan.Description = request.Description;
                if (request.Active)
                {
                    plan.Activate();
                }
                else
                {
                    plan.Deactivate();
                }

                _trainingPlanService.UpdateTrainingPlan(plan);
                return Ok(new { message = "Training plan updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error updating training plan: {ex.Message}" });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _trainingPlanService.DeleteTrainingPlan(id);
                return Ok(new { message = "Training plan deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error deleting training plan: {ex.Message}" });
            }
        }
    }
}
