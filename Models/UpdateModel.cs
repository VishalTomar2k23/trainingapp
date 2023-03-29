﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class UpdateModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public IFormFile File { get; set; }
    }
}
