using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Session und Cache konfigurieren
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

var app = builder.Build();

app.UseSession();

// Zugriffsschutz für app.htm
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/app.htm"))
    {
        var authenticated = context.Session.GetBool("authenticated") ?? false;
        if (!authenticated)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Zugriff verweigert");
            return;
        }
    }
    await next();
});

app.UseStaticFiles();

// Login-Logik auf /login
app.MapGet("/login", async context =>
{
    var mt = context.Request.Query["mt"];

    if (mt == "AppChallenge")
    {
        var challenge = context.Session.GetString("challenge");
        if (string.IsNullOrEmpty(challenge))
        {
            challenge = GenerateRandomString(8);
            context.Session.SetString("challenge", challenge);
        }

        await context.Response.WriteAsJsonAsync(new { mt = "AppChallengeResult", challenge });
    }
    else if (mt == "AppLogin")
    {
        var app = context.Request.Query["app"];
        var domain = context.Request.Query["domain"];
        var sip = context.Request.Query["sip"];
        var guid = context.Request.Query["guid"];
        var dn = context.Request.Query["dn"];
        var info = context.Request.Query["info"];
        var digest = context.Request.Query["digest"];
        var challenge = context.Request.Query["challenge"];
        var appPwd = "pwd";

        var data = $"{app}:{domain}:{sip}:{guid}:{dn}:{info}:{challenge}:{appPwd}";
        using var sha = SHA256.Create();
        var hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();

        if (hash == digest)
        {
            context.Session.SetBool("authenticated", true);
            await context.Response.WriteAsJsonAsync(new { mt = "AppLoginResult", ok = true });
        }
        else
        {
            await context.Response.WriteAsJsonAsync(new { mt = "AppLoginResult", ok = false });
        }
    }
    else
    {
        await context.Response.WriteAsync("Unbekannter Request");
    }
});

app.Run("https://0.0.0.0:8181");

// Hilfsmethoden
static string GenerateRandomString(int length)
{
    const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
    var result = new char[length];
    using var rng = RandomNumberGenerator.Create();
    var data = new byte[length];
    rng.GetBytes(data);
    for (int i = 0; i < length; i++) result[i] = chars[data[i] % chars.Length];
    return new string(result);
}

public static class SessionExtensions
{
    public static void SetBool(this ISession session, string key, bool value) =>
        session.SetString(key, value ? "1" : "0");

    public static bool? GetBool(this ISession session, string key) =>
        session.GetString(key) switch
        {
            "1" => true,
            "0" => false,
            _ => null
        };
}
