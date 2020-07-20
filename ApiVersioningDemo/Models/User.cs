using System;

namespace ApiVersioningDemo.Models
{
    public class User
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}