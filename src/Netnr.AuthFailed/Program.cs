using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "JWT_OR_COOKIE";
    options.DefaultChallengeScheme = "JWT_OR_COOKIE";
}).AddCookie(options =>
{
    options.Cookie.Name = ".auth";
    options.Events.OnRedirectToAccessDenied = options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Headers.Remove(HeaderNames.Location);
        context.Response.StatusCode = 401;

        return Task.CompletedTask;
    };
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "Netnr",
        ValidAudience = "netnr",
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,//是否验证失效时间
        LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters tokenValidationParameters) => expires != null && expires > DateTime.UtcNow,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.Empty.ToString()))
    };
}).AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
{
    // Add this to allow both Cookies and Bearer Tokens , using default scheme names
    options.ForwardDefaultSelector = context =>
    {
        context.Response.Headers.AccessControlExposeHeaders = "www-authenticate,x-auth-type,x-refresh-token,date";

        string authorization = context.Request.Headers[HeaderNames.Authorization];
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            //jwt
            context.Response.Headers["x-auth-type"] = "JWT";
            return JwtBearerDefaults.AuthenticationScheme;
        }

        //cookie
        context.Response.Headers["x-auth-type"] = "Cookie";
        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
});

var app = builder.Build();

app.UseExceptionHandler(options =>
{
    options.Run(async context =>
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("-1");
    });
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
