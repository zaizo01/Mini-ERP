using AutoMapper;
using EmailService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApiPractices.DTOs;
using WebApiPractices.Entities;

namespace WebApiPractices.Controllers
{
    [Route("api/Accounts/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;
        private readonly IEmailSender emailSender;

        public AccountsController(UserManager<IdentityUser> userManager, IConfiguration configuration,
            SignInManager<IdentityUser> signInManager, IMapper mapper, ApplicationDbContext context,
            IEmailSender emailSender
           )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.context = context;
            this.emailSender = emailSender;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseAuthentication>> UserRegister(UserCredentials user)
        {
            var userExist = await context.Users.AnyAsync(userDB => userDB.Email == user.Email);
            if (userExist) return BadRequest($"The user {user.Email} exist.");
            var userDB = new IdentityUser { UserName = user.Email, Email = user.Email };
            var result = await userManager.CreateAsync(userDB, user.Password);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(userDB);
            var confirmationLink = Url.Action(nameof(UserConfirmEmail), "Accounts", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { userDB.Email }, "Confirmation Link", confirmationLink, null);
            await emailSender.SendEmailAsync(message);

            if (result.Succeeded)
            {
                await userManager.AddClaimAsync(userDB, new Claim("Admin", "1"));
                return await BuildToken(user, userDB.Id);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet]
        public async Task<ActionResult> UserConfirmEmail(string token, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return NotFound($"The user {email} not exist.");

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseAuthentication>> UserLogin(UserCredentials user)
        {
            var loginResult = await signInManager
                .PasswordSignInAsync(user.Email, user.Password, isPersistent: false, lockoutOnFailure: false);

            var userDB = await userManager.FindByEmailAsync(user.Email);

            if (userDB.EmailConfirmed)
            {
                if (loginResult.Succeeded)
                {
                    return await BuildToken(user, userDB.Id);
                }
                else
                {
                    return BadRequest("User or Password incorrect.");
                }
            }

            return BadRequest($"The {user.Email} has not been confirmed. Please check your email.");
        }

        [HttpPost]
        public async Task<ActionResult> UserForgotPassword(UserForgotPasswordDTO userForgotPasswordDTO)
        {
            var user = await userManager.FindByEmailAsync(userForgotPasswordDTO.Email);
            if (user == null) return NotFound($"The {userForgotPasswordDTO.Email} not exist.");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callback = Url.Action(nameof(ResetPassword), "Accounts", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email }, "Reset password token", callback, null);
            await emailSender.SendEmailAsync(message);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword([FromForm] UserResetPasswordDTO userResetPasswordDTO)
        {

            var user = await userManager.FindByEmailAsync(userResetPasswordDTO.Email);
            if (user == null) return NotFound($"The user {userResetPasswordDTO.Email} not exist.");
            var token = userResetPasswordDTO.Token.Replace("%2F", "/").Replace("%2B", "+");
            var resetPassResult = await userManager.ResetPasswordAsync(user, token, userResetPasswordDTO.Password);


            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    return BadRequest($"{error.Code} - {error.Description}");
                }
            }
            return Ok("the password has been successfully reset.");
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UserPut([FromBody] UserPutDTO userPutDTO, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound($"The user {userPutDTO.Email} not exist.");
            user.Email = userPutDTO.Email;
            user.UserName = userPutDTO.UserName;

            await userManager.UpdateAsync(user);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> UserDelete(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("The user not exist.");
            await userManager.DeleteAsync(user);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
        [HttpGet("{userId}")]
        public async Task<ActionResult<UsersGetRolesDTO>> GetUserById(string userId)
        {
            var user = await (from c in context.UserClaims
                              join u in context.Users on c.UserId equals u.Id
                              where u.Id == userId
                              select new UsersGetRolesDTO { Id = u.Id, Email = u.Email, ClaimRol = c.ClaimType }).FirstOrDefaultAsync();

            if (user == null) return BadRequest("This user not exist");
            return mapper.Map<UsersGetRolesDTO>(user);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<UserGetDTO>>> UserList()
        {
            var usersList = await context.Users.ToListAsync();
            return mapper.Map<List<UserGetDTO>>(usersList);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<UsersGetRolesDTO>>> UserGetListClaimRol()
        {
            var userListWithRolClaim = await (from c in context.UserClaims
                                              join u in context.Users on c.UserId equals u.Id
                                              select new UsersGetRolesDTO { Id = u.Id, Email = u.Email, ClaimRol = c.ClaimType }).ToListAsync();

            return mapper.Map<List<UsersGetRolesDTO>>(userListWithRolClaim);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
        public async Task<ActionResult> PostUserClaim(UserPostClaimDTO userPostClaimDTO)
        {

            var userClaim = await (from c in context.UserClaims
                                   join u in context.Users on c.UserId equals u.Id
                                   where u.Email == userPostClaimDTO.Email
                                   select new { Id = u.Id, Email = u.Email, ClaimRol = c.ClaimType }).FirstOrDefaultAsync();

            if (!String.IsNullOrEmpty(userClaim.ClaimRol)) return BadRequest($"The {userPostClaimDTO.Email} alredy has a role assigned.");

            var user = await userManager.FindByEmailAsync(userPostClaimDTO.Email);

            if (user == null) return BadRequest("This user not exist");
            await userManager.AddClaimAsync(user, new Claim($"{userPostClaimDTO.UserClaim}", "1"));
            return NoContent();
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
        [HttpPut("{userId}")]
        public async Task<ActionResult> UserPutClaimRol([FromBody] UserPutClaimRolDTO userPutDTO, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound($"The user not exist.");

            var claim = await context.UserClaims.FirstOrDefaultAsync(x => x.UserId == userId);
            await userManager.ReplaceClaimAsync(user, new Claim($"{claim.ClaimType}", "1"), new Claim($"{userPutDTO.UserClaim}", "1"));
            return NoContent();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
        public async Task<ActionResult> RemoveUserClaim(UserDeleteClaimDTO userDeleteClaimDTO)
        {
            var user = await userManager.FindByEmailAsync(userDeleteClaimDTO.Email);
            var claim = await context.UserClaims.FirstOrDefaultAsync(cla => cla.UserId == user.Id);
            await userManager.RemoveClaimAsync(user, new Claim($"{claim.ClaimType}", "1"));
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseAuthentication>> RenewToken()
        {
            var emailUserClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var userEmail = emailUserClaim.Value;
            var idUserClaim = HttpContext.User.Claims.Where(claim => claim.Type == "id").FirstOrDefault();
            var userId = idUserClaim.Value;

            var userCredentials = new UserCredentials
            {
                Email = userEmail
            };
            return await BuildToken(userCredentials, userId);
        }

        [HttpPost]
        public async Task<ActionResult> SendEmail()
        {
            // if you want to send attachements in email use this code. 
            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
            if (files == null) return BadRequest("The files is required");

            var message = new Message(new string[] { $"e.zaizortega@gmail.com" }, "Test Email", "This a content test email service.", files);
            await emailSender.SendEmailAsync(message);

            return Ok();
        }

        private async Task<ResponseAuthentication> BuildToken(UserCredentials user, string userId)
        {
            var userDB = await userManager.FindByEmailAsync(user.Email);
            var claimsDB = await userManager.GetClaimsAsync(userDB);

            var claims = new List<Claim>()
            {
                new Claim("email", user.Email),
                new Claim("id", userId),
            };

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyJwt"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationToken = DateTime.UtcNow.AddMinutes(30);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expirationToken, signingCredentials: credentials);

            return new ResponseAuthentication()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expirationToken
            };
        }
    }
}
