﻿using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IProfileService
    {
        Profile CheckPassword(LoginModel loginModel);

        Profile RegisterUser(RegisterModel regModel);

        Profile UpdateUser(UpdateModel updateModel, string username);

        string GetImage(string username);

        List<profileDTO> getAll();

        List<profileDTO> GetProfileDTOs(string s);

        Profile getUser(string userName);
    }
}
