using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Buddha.Components;
using Buddha.Components.Account;
using Buddha.Data;
using Buddha.Helpers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

const string corsapp = "corsapp";
builder.Services.AddOpenApi();
builder.WebHost.UseUrls("https://localhost:58261");

builder.Services.AddCors(options => options.AddPolicy(corsapp,
    policy => policy.WithOrigins(Helper.AllowOrigins())
        .AllowAnyMethod()
        .AllowAnyHeader()
));

builder.Services.AddControllers();

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();
builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDbContext<BuddhaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BuddhaConnection")));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();
app.UseCors(corsapp);

if (app.Environment.IsDevelopment())
{
    // --> UI, https://localhost:58261/scalar/v1
    // --> Json, https://localhost:58261/openapi/v1
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();

}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Buddha.Client._Imports).Assembly);


app.MapAdditionalIdentityEndpoints();

app.MapControllers();

app.Run();
