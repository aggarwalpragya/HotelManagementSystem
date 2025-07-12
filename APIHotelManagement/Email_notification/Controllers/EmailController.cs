using Email_notification.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Email_notification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)

        {

            _emailService = emailService;

        }

        [HttpPost("send")]

        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)

        {

            if (emailRequest == null)

                return BadRequest("Invalid email request.");

            var result = await _emailService.SendEmailAsync(emailRequest);

            if (result)

                return Ok(new { message = "Email sent successfully!" });

            else

                return StatusCode(500, new { message = "Failed to send email." });

        }
    }
}
