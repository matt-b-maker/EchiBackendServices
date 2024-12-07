using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace EchiBackendServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : Controller
    {
        [HttpPost]
        public IActionResult SendEmail()
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("myusername@gmail.com", "mypwd"),
                EnableSsl = true
            };
            client.Send("myusername@gmail.com", "myusername@gmail.com", "test", "testbody");

            return Ok();
        }
    }
}
