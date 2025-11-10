using System.Security.Claims;
using common_extensions.Observability;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDefaultObservability(builder.Configuration, builder.Environment);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"]
                            ?? "http://localhost:8085/realms/Elsewhere";
        options.Audience = builder.Configuration["Authentication:Audience"] ?? "elsewhere-web";
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "preferred_username",
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Travelers", policy =>
        policy.RequireRole("traveler"));

    options.AddPolicy("CreatorsOnly", policy =>
        policy.RequireRole("traveler", "creator"));
});

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async transformContext =>
        {
            var user = transformContext.HttpContext.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var sub = user.FindFirst("sub")?.Value
                          ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var email = user.FindFirst("preferred_username")?.Value
                         ?? user.FindFirst(ClaimTypes.Email)?.Value;

                var roleClaims = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

                if (!string.IsNullOrEmpty(sub))
                    transformContext.ProxyRequest.Headers.Add("X-User-Id", sub);

                if (!string.IsNullOrEmpty(email))
                    transformContext.ProxyRequest.Headers.Add("X-User-Email", email);

                if (roleClaims.Count > 0)
                    transformContext.ProxyRequest.Headers.Add("X-User-Roles", string.Join(",", roleClaims));
            }

            await Task.CompletedTask;
        });
    });

var app = builder.Build();

app.UseAuthentication();
app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("Authorization", out var header))
    {
        Console.WriteLine($"[Gateway] Incoming Authorization: {header}");
    }
    else
    {
        Console.WriteLine("[Gateway] No Authorization header found");
    }

    await next();
});
app.UseAuthorization();
app.MapReverseProxy();

app.Run();