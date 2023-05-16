﻿using ChatApp.Business.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Context.EntityClasses
{
    public class Profile
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ProfileType ProfileType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string? ImageUrl { get; set; }

        [Display(Name ="Designation")]
        public int DesignationID { get; set; }

        [ForeignKey("DesignationID")]
        public virtual DesignationEntity Designation { get; set; }

        public int StatusID { get; set; }

        [ForeignKey("StatusID")]
        public virtual Status Status { get; set; }

        public bool IsActive { get; set; }

    }
}
