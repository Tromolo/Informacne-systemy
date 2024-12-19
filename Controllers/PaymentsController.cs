using Microsoft.AspNetCore.Mvc;
using System;
using VIS_projekt.Services;
using VIS_projekt.Controllers.Models;

namespace VIS_projekt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        [HttpPost("process")]
        public IActionResult ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            if (request == null || request.UserId <= 0 || request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Type))
            {
                return BadRequest(new { message = "Neplatné údaje o platbe." });
            }

            try
            {
                _paymentService.ProcessPayment(request.UserId, request.Type, request.Amount);
                return Ok(new { message = "Platba bola úspešne vykonaná." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Chyba pri vykonávaní platby: {ex.Message}" });
            }
        }


        [HttpGet("user/{userId}")]
        public IActionResult GetPaymentHistory(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Neplatné ID používateľa." });

            try
            {
                var payments = _paymentService.GetPaymentsByUserId(userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Chyba pri načítaní histórie platieb: {ex.Message}" });
            }
        }


        [HttpGet("trainer/{trainerId}")]
        public IActionResult GetPaymentsForTrainer(int trainerId)
        {
            if (trainerId <= 0)
                return BadRequest(new { message = "Neplatné ID trénera." });

            try
            {
                var payments = _paymentService.GetPaymentsByTrainerId(trainerId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Chyba pri načítaní histórie platieb pre trénera: {ex.Message}" });
            }
        }

        [HttpPost("csv/process")]
        public IActionResult ProcessCsvPayment([FromBody] ProcessPaymentRequest request)
        {
            if (request == null || request.UserId <= 0 || request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Type))
            {
                return BadRequest(new { message = "Invalid payment data." });
            }

            try
            {
                _paymentService.ProcessPayment(request.UserId, request.Type, request.Amount);
                return Ok(new { message = "Payment successfully processed." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error processing payment: {ex.Message}" });
            }
        }

    }
}
