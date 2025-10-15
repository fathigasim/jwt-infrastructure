using JwtInfrastructure.Data;
using JwtInfrastructure.Resources;
using JwtInfrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserContext>(opts =>
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:infrajwt"]));
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// add this line if using validation/localizer
builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
const string defaultCulture = "en";

//var supportedCultures = new[]
//{
//    new CultureInfo(defaultCulture),
//    new CultureInfo("ar")
//};
var supportedCultures = new[] { "en", "ar" };
builder.Services.Configure<RequestLocalizationOptions>(options => {
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures.Select(c=>new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
});

// Add services to the container.
//builder.Services.AddSingleton<CommonLocalizationService>();
//builder.Services.AddRazorPages().AddViewLocalization().AddDataAnnotationsLocalization(options =>
//{
//    // 🔧 Redirect all validation messages to CommonResources
//    options.DataAnnotationLocalizerProvider = (type, factory) =>
//        factory.Create("CommonResources", typeof(Program).Assembly.FullName!);
//});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
    ValidAudience = builder.Configuration["JwtSettings:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
};
});
var allowFrontEnd= "allowfront";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowFrontEnd,
          policy =>
          {
              policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
             // .WithExposedHeaders("X-Pagination");
            
          });

});
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
}).AddViewLocalization().AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (type, factory) =>
    {
        // Specify the shared resource type for DataAnnotations
        var assemblyName = new System.Reflection.AssemblyName(typeof(CommonResources).Assembly.FullName!);
        return factory.Create("CommonResources", assemblyName.Name!);

    };
});
//builder.Services.AddControllers().AddNewtonsoftJson(options =>
//{
//    options.SerializerSettings.ContractResolver = new DefaultContractResolver
//    {
//        NamingStrategy = new CamelCaseNamingStrategy
//        {
//            ProcessDictionaryKeys = true,
//            OverrideSpecifiedNames = false
//        }
//    };
//}); 
builder.Services.AddScoped<ITokenService, TokenService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(allowFrontEnd);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// 🌍 Add this to respect Accept-Language
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);
//app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
