using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ================= AUTH =================

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.email == dto.email);

            if (exists)
                return BadRequest("Email already exists");

            var user = new User
            {
                username = dto.username,
                email = dto.email,
                password_hash = BCrypt.Net.BCrypt.HashPassword(dto.password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Register success" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email == dto.email);

            if (user == null)
                return Unauthorized("User not found");

            bool valid = BCrypt.Net.BCrypt.Verify(dto.password, user.password_hash);

            if (!valid)
                return Unauthorized("Wrong password");

            var token = GenerateJwt(user);

            return Ok(new { token });
        }

        // ================= USER =================

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();

            var user = await _context.Users
                .Where(u => u.user_id == userId)
                .Select(u => new
                {
                    user_id = u.user_id,
                    avatar = u.Avatar,
                    username = u.username,
                    email = u.email,
                    created_at = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Không tìm thấy user" });

            return Ok(user);
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = GetUserId();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.user_id == userId);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy user" });

            user.Avatar = dto.avatar;
            user.username = dto.username;
            user.email = dto.email;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thành công" });
        }

        // ================= ACCOUNT =================

        [Authorize]
        [HttpPost("open-account")]
        public async Task<IActionResult> OpenAccount([FromBody] Account dto)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(dto.typeAccount))
                return BadRequest(new { message = "Thiếu typeAccount" });

            var accountId = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            var account = new Account
            {
                account_id = accountId,
                user_id = userId,
                balance = 10000,
                leverage = dto.leverage == 0 ? 100 : dto.leverage,
                typeAccount = dto.typeAccount
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Mở tài khoản thành công!",
                account_id = accountId
            });
        }

        [Authorize]
        [HttpGet("account")]
        public async Task<IActionResult> GetAccounts()
        {
            var userId = GetUserId();

            var accounts = await _context.Accounts
                .Where(a => a.user_id == userId)
                .Select(a => new
                {
                    account_id = a.account_id,
                    balance = a.balance,
                    used_margin = a.used_margin,
                    leverage = a.leverage,
                    typeAccount = a.typeAccount
                })
                .ToListAsync();

            return Ok(accounts);
        }

        // ================= HELPER =================

        private int GetUserId()
        {
            var id = User.FindFirst("id")?.Value;
            return int.Parse(id);
        }

        private string GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", user.user_id.ToString()),
                new Claim("email", user.email)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}