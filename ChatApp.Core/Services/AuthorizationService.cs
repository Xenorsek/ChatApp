using ChatApp.Common.ConfigurationHelpers;
using ChatApp.Core.Interfaces;
using ChatApp.Infrastructure.DataAccess.Entities;
using ChatApp.Infrastructure.Models;
using ChatApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Core.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly JwtBearerConfig _jwtBearerConfig;
        private readonly IIdentityUserRepository _identityUserRepository;

        public AuthorizationService(IOptions<JwtBearerConfig> jwtBearerConfig, IIdentityUserRepository identityUserRepository)
        {
            _jwtBearerConfig = jwtBearerConfig.Value;
            _identityUserRepository = identityUserRepository;
        }

        public async Task<UserInfo> Login(string userProvider, string password)
        {
            UserInfo? userInfo = await _identityUserRepository.LoginUser(userProvider, password);
            if (userInfo == null)
            {
                throw new InvalidCredentialException("Wrong credentials");
            }

            return userInfo;
        }

        public async Task<bool> Register(string username, string email,  string password)
        {
           bool SuccessfulRegister = await _identityUserRepository.RegisterUser(username, email, password);
            if (SuccessfulRegister is false)
            {
                throw new Exception("Failed while creating new user");
            }
            return true;
        }
    }
}
