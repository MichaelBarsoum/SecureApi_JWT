using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Secure_Api_Using_JWT.DbContext.Identity;
using Secure_Api_Using_JWT.Helpers;
using Secure_Api_Using_JWT.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Secure_Api_Using_JWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jWT;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(UserManager<ApplicationUser> userManager, JWT jWT, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jWT = jWT;
            _configuration = configuration;
            _roleManager = roleManager;
        }
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = $" This Email {model.Email} is Already Registered " };
            if (await _userManager.FindByEmailAsync(model.UserName) is not null)
                return new AuthModel { Message = $" This Name {model.UserName} is Already Registered " };
            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                lastName = model.LastName,
                Email = model.Email,
                PasswordHash = model.Password
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                    errors += $"{error.Description} , ";
                return new AuthModel() { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, UserRoles.USER.ToString());
            var jwtsecuritytoken = await CreateJwtToken(user);
            return new AuthModel()
            {
                Email = user.Email,
                ExpirationTime = jwtsecuritytoken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtsecuritytoken),
                UserName = user.UserName
            };
        }
        public async Task<AuthModel> LoginAsync(TokenRequestModel model)
        {
            var authmodel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authmodel.Message = " Your Email Or Password is Incorrect ";
                return authmodel;
            }
            var jwtsecuritytoken = await CreateJwtToken(user);
            var RolesList = await _userManager.GetRolesAsync(user);
            authmodel.Email = user.Email;
            authmodel.ExpirationTime = jwtsecuritytoken.ValidTo;
            authmodel.IsAuthenticated = true;
            authmodel.Roles = new List<string> { "User" };
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(jwtsecuritytoken);
            authmodel.UserName = user.UserName;
            return authmodel;
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var UserClaims = await _userManager.GetClaimsAsync(user);
            var Roles = await _userManager.GetRolesAsync(user);
            var RoleClaims = new List<Claim>();
            foreach (var Role in Roles)
                RoleClaims.Add(new Claim("Role", Role));
            var Claims = new[]
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub ,user.UserName),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email ,user.Email),
                new Claim("UserId" ,user.Id)
            }.Union(UserClaims).Union(RoleClaims);
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));
            var SigningCredintials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken
                (
                    issuer: _jWT.Issuer,
                    audience: _jWT.Audience,
                    claims: Claims,
                    expires: DateTime.Now.AddDays(_jWT.DurationInDays),
                    signingCredentials: SigningCredintials
                );
            return jwtSecurityToken;
        }
        public async Task<string> AddRoleAsync(RoleModel model)
        {
            var user = new ApplicationUser();
            if (await _userManager.FindByIdAsync(model.UserID) is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid User Or Role";
            if (!await _userManager.IsInRoleAsync(user, model.Role))
                return "User already Assigned To This Role ";
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? string.Empty : " Somthing Went Wrong ";
        }
    }
}