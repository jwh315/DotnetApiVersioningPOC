using ApiVersioningDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiVersioningDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public User Get()
        {
            return new User();
        }

        [HttpPost]
        public User Post([FromBody] User user)
        {
            return user;
        }
    }
}