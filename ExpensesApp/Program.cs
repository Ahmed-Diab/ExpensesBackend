using Expenses.Core;
using Expenses.Core.Moduels;
using Expenses.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
// Add services to the container.


builder.Services.AddAutoMapper(typeof(Program));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddIdentityCore<User>(opt => {
    opt.Password.RequireDigit = false;
    opt.Password.RequiredLength = 4;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ذ ض ص ث ق ف غ ع ه خ ح ج د ط ك م ن ت ا ل ب ي س ش ئ ء ؤ ر لا ى ة و ز ظ أ";
}).AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders().AddSignInManager<SignInManager<User>>();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        //ValidateIssuerSigningKey = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("AppSettings:SecretKey").Value)),
        ValidateIssuer = false,
        ValidateAudience = false,
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy",
        builder => builder
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true)
            .AllowAnyHeader());
}); ;
builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
//    options =>
//{
//    var policy = new AuthorizationPolicyBuilder()
//                .RequireAuthenticatedUser()
//                .Build();
//    options.Filters.Add(new AuthorizeFilter(policy));
//}).AddDataAnnotationsLocalization().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
//.AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("MyCorsPolicy");
app.MapControllers();

app.Run();
