using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace APIAuthenticationAuthorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly DBContext db;
        public UserController(DBContext _db)
        {
            this.db = _db;
        }



        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (db.Users.Any(user=> user.Email==request.Email))
            {
                return BadRequest("User Already Exists...");
            }

            CreatePasswordHash(request.Password, 
                out byte[] passwordHash, 
                out byte[] passwordSalt);

            var User = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken=CreateRandomToken()
            };

            db.Users.Add(User);
            await db.SaveChangesAsync();

            return Ok($"User Successfully Registered...Your Verified Token is { User.VerificationToken}");

        }

        private void CreatePasswordHash(string password,
             out byte[] passwordHash,
                out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }


        private bool VerifyPasswordHash(string password,
        byte[] passwordHash,
        byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = await db.Users.FirstOrDefaultAsync(user=> user.Email == request.Email);

            if (user == null) 
            {
                return BadRequest("User Not Found...");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is Incorrect...");
            }

            if (user.VerifiedAt==null)
            {
                return BadRequest("Not Verified...");
            }

            return Ok($"Welcome! {user.Email}");
        }



        [HttpPost("Verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = await db.Users.FirstOrDefaultAsync(user => user.VerificationToken == token);

            if (user == null)
            {
                return BadRequest("Invalid Token...");
            }

            user.VerifiedAt= DateTime.Now;
            await db.SaveChangesAsync();

            return Ok("User Verified...");
        }


        [HttpPost("Forgot Password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await db.Users.FirstOrDefaultAsync(user => user.Email == email);

            if (user == null)
            {
                return BadRequest("User Not Found...");
            }

            user.PasswordResetToken = CreateRandomToken();

            user.ResetTokenExpires= DateTime.Now.AddMinutes(5);
            await db.SaveChangesAsync();

            return Ok($"You May Now Reset Your Password Within 5 Minutes.. <br/>Your Reset Password Token is {user.PasswordResetToken}");
        }



        [HttpPost("Reset Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await db.Users.FirstOrDefaultAsync(user => user.PasswordResetToken == request.Token);

            if (user == null || user.ResetTokenExpires<DateTime.Now)
            {
                return BadRequest("Invalid Token...");
            }


            CreatePasswordHash(request.Password,
                 out byte[] passwordHash,
                 out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt=passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await db.SaveChangesAsync();

            return Ok("Password Successfully Reset...");
        }
    }
}
