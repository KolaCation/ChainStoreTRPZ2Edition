﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChainStore.Domain.Identity;

namespace ChainStore.DataAccessLayer.Identity
{
    public interface IAuthenticationService
    {
        Task<bool> IsLoggedIn();
        Task<bool> Login(string email, string password);
        Task<bool> Register(string email, string password, string confirmPassword);


    }
}
