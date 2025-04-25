using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PicHub.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var userPoolClientId = builder.Configuration["USER_POOL_CLIENT_ID"];
var userPoolClientSecret = builder.Configuration["USER_POOL_CLIENT_SECRET"];
var jwtAuthority = builder.Configuration["JWT_AUTHORITY"];

builder.Services.AddScoped<IAmazonCognitoIdentityProvider>(_ =>
    new AmazonCognitoIdentityProviderClient(region:Amazon.RegionEndpoint.EUNorth1));

builder.Services.AddScoped<IAmazonS3>(_ =>
    new AmazonS3Client(region: Amazon.RegionEndpoint.EUNorth1));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
    {
        o.Authority = jwtAuthority;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = userPoolClientId,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "cognito:groups"    // check the role claim
        };
        // o.MapInboundClaims = false;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<CognitoService>();
builder.Services.AddScoped<S3Service>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMemoryCache();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a valid access token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();  // commented out for development purposes

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
