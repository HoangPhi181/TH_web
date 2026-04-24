using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 1. DATABASE
// =======================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// =======================
// 2. JWT AUTH (Đã sửa lỗi Null)
// =======================
// Lấy Key và kiểm tra xem có tồn tại trong appsettings.json không
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    // Nếu thiếu Key, in cảnh báo ra Console thay vì để sập App
    Console.WriteLine("❌ CẢNH BÁO: Jwt:Key chưa được cấu hình trong appsettings.json!");
    jwtKey = "Temporary_Fallback_Key_For_Development_Only_32_Chars"; 
}

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// =======================
// 3. CORS (Cấu hình chuẩn)
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Chỉ định rõ Port của React Vite
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Cho phép gửi kèm Token/Cookie nếu cần
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// =======================
// 4. PIPELINE (Thứ tự chuẩn của Microsoft)
// =======================

// Luôn để Developer Exception Page đầu tiên ở môi trường Dev
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

// CORS PHẢI nằm sau UseRouting và TRƯỚC Auth
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Urls.Add("http://localhost:5059");
app.Run();