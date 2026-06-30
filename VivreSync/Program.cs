using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VivreSync.Allocations.Repositories;
using VivreSync.Allocations.Services;
using VivreSync.Authentication.Repositories;
using VivreSync.Authentication.Security;
using VivreSync.Authentication.Services;
using VivreSync.HR.Repositories;
using VivreSync.HR.Services;
using VivreSync.Model.Entities;
using VivreSync.Projects.Repositories;
using VivreSync.Projects.Services;
using VivreSync.Shared.Exceptions;
using VivreSync.Structure.Data;
using VivreSync.Timesheets.Repositories;
using VivreSync.Timesheets.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().
    AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.UnmappedMemberHandling =
            JsonUnmappedMemberHandling.Disallow;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IPasswordHasher<Users>, PasswordHasher<Users>>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<ISkillService, SkillService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

builder.Services.AddScoped<IMilestoneRepository, MilestoneRepository>();
builder.Services.AddScoped<IMilestoneService, MilestoneService>();

builder.Services.AddScoped<IAllocationRepository, AllocationRepository>();
builder.Services.AddScoped<IAllocationService, AllocationService>();

builder.Services.AddScoped<ITimesheetsRepository, TimesheetRepository>();
builder.Services.AddScoped<ITimesheetsService, TimesheetsService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = "Login Required"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },

        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = "Access denied. You do not have permission to access it"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    };
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Where(x => x.Value != null && x.Value.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(error =>
            {
                var fieldName = x.Key.Replace("$.", "").Replace("dto.", "").Replace("DTO.", "").Trim();

                if (string.IsNullOrWhiteSpace(fieldName) || fieldName == "dto")
                    fieldName = "Request body";

                if (error.ErrorMessage.Contains("The JSON value could not be converted"))
                    return $"{fieldName} has invalid data type";

                if (error.ErrorMessage.Contains("The dto field is required"))
                    return "Request body is required";

                return error.ErrorMessage;
            })).ToList();

        return new BadRequestObjectResult(new
        {
            message = "Validation failed",
            errors = errors
        });
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";
    var isSwagger =path.Contains("/swagger") || path.Contains("/swagger/v1/swagger.json");
    if (isSwagger)
    {
        await next();
        return;
    }

    var isLoggedIn = context.User.Identity?.IsAuthenticated == true;
    if (!isLoggedIn)
    {
        await next();
        return;
    }

    var isLogin = path.Contains("/api/auth/login");
    if (!isLogin)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tokenVersionClaim = context.User.FindFirst("tokenVersion")?.Value;
        if (!int.TryParse(userIdClaim, out var userId) ||
            !int.TryParse(tokenVersionClaim, out var tokenVersionFromToken))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Invalid token"
            });
            return;
        }

        using var scope = context.RequestServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null || !user.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Invalid or inactive user"
            });
            return;
        }
        if (user.TokenVersion != tokenVersionFromToken)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Token expired due to password change. Please login again."
            });
            return;
        }
    }

    var mustChangePassword = context.User.FindFirst("passwordChangeRequired")?.Value == "True";
    var isChangePassword = path.Contains("/api/auth/changepassword") || path.Contains("/api/auth/change-password");
    var allowedApi = isLogin || isChangePassword;
    if (mustChangePassword && !allowedApi)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsJsonAsync(new
        {
            message = "Password change is required before accessing the system."
        });
        return;
    }
    await next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();