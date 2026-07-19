using Microsoft.EntityFrameworkCore;
using MeetSpot.Infrastructure.Data;
using System.Text;
using FluentValidation;
using MeetSpot.API.BackgroundServices;
using MeetSpot.API.Hubs;
using MeetSpot.API.Middleware;
using MeetSpot.API.Services;
using MeetSpot.Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MeetSpot.Core.Interfaces;
using MeetSpot.Core.Mappings;
using MeetSpot.Infrastructure.Services;
using Microsoft.AspNetCore.Http.Features;
using Scalar.AspNetCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MeetSpotAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "MeetSpot_";
});

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key is missing. Set it via environment variables or user secrets.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingNotificationService, BookingNotificationService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IHeroSlideService, HeroSlideService>();
builder.Services.AddScoped<ICompanyRequestService, CompanyRequestService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddHostedService<BookingAutoReleaseService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<MeetSpot.Core.Validators.RegisterValidator>();  
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});


builder.Services.AddControllers();

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string>()?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb", policy =>
    {
        if (corsOrigins.Length > 0)
        {
            policy.WithOrigins(corsOrigins);
        }
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MeetSpotAppDbContext>();
    
    if (!context.Users.Any(u => u.Role == UserRole.Admin))
    {
        var adminEmail = builder.Configuration["AdminSeed:Email"] ?? "admin@meetspot.local";
        var adminPassword = builder.Configuration["AdminSeed:Password"];
        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException("AdminSeed:Password is missing. Set a strong initial admin password via environment variables or user secrets.");
        }

        context.Users.Add(new User
        {
            FirstName = "Admin",
            LastName = "MeetSpot",
            Email = adminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Role = UserRole.Admin,
            Type = UserType.Regular,
            IsEmailConfirmed = true,
            IsActive = true
        });
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseStaticFiles();
app.UseCors("AllowWeb");

// app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<BookingHub>("/hubs/booking");

app.MapControllers();

app.Run();
