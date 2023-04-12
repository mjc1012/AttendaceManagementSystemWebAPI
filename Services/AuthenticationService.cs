﻿using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace AttendaceManagementSystemWebAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly DataContext _context;
        public AuthenticationService(DataContext context)
        {
            _context = context;
        }


        public string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new();
            if (password.Length < 8)
            {
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            }
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
            {
                sb.Append("Password should contain atleast one Uppercase Letter, one Lowercase Letter and one Number" + Environment.NewLine);
            }
            if (!Regex.IsMatch(password, "[~,',!,@,#,$,%,^,&,*,(,),-,_,+,=,{,},\\[,\\],|,/,\\,:,;,\",`,<,>,,,.,?]"))
            {
                sb.Append("Password should contain contain atleast one special character" + Environment.NewLine);
            }
            return sb.ToString();
        }

        public string CreateJwt(Employee employee)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryveryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, employee.EmployeeRole.Name),
                new Claim(ClaimTypes.Name, employee.EmployeeIdNumber)
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(1),
                SigningCredentials = credentials,
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken)
        {
            var key = Encoding.ASCII.GetBytes("veryveryverysecret.....");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(expiredToken, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("This is an invalid token");
            }
            else
            {
                return principal;
            }
        }

        public async Task<bool> EmployeeRefreshTokenExists(string refreshToken)
        {
            try
            {
                return await _context.Employees.AnyAsync(p => p.RefreshToken == refreshToken.Trim());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes).Replace("/", "!").Replace("\\", "!");

            var tokenInUser = await EmployeeRefreshTokenExists(refreshToken);

            if (tokenInUser)
            {
                return await CreateRefreshToken();
            }
            return refreshToken;
        }

        public async Task<Employee> saveTokens(Employee employee, string accessToken, string refreshToken)
        {
            try
            {
                employee.AccessToken = accessToken;
                employee.RefreshToken = refreshToken;
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Employee> saveTokens(Employee employee, string accessToken, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            try
            {
                employee.AccessToken = accessToken;
                employee.RefreshToken = refreshToken;
                employee.RefreshTokenExpiryTime = refreshTokenExpiryTime;
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
