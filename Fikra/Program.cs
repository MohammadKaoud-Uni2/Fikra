using Fikra.Hubs;
using Fikra.Mapper;
using Fikra.Service;
using Fikra.Service.Implementation;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using QuestPDF.Infrastructure;
using SparkLink.Data;
using SparkLink.Models.Identity;
using SparkLink.Service;
QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = true;

}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddSignalR();
builder.Services.AddRepoService();
builder.Services.JwtRegistering(builder.Configuration);
builder.Services.RegisterEmail(builder.Configuration);
builder.Services.RegKeyService(builder.Configuration);
builder.Services.AutoMapReg();
builder.Services.AddHttpClient();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.SetIsOriginAllowed(origin => true);
    options.AllowCredentials();
    options.WithExposedHeaders();
});
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
    var userManager=scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

    await IDentitySeeder.seedRoles(roleManager,userManager);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "images", "profilePictures")),
    RequestPath = "/images/profilePictures"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "contracts")),
    RequestPath = "/contracts"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "cvs")),
    RequestPath = "/cvs"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "complaints")),
    RequestPath = "/complaints"
});

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
   
    endpoints.MapControllers();
    endpoints.MapHub<ContractHub>("Hubs/ContractHub");
    endpoints.MapHub<GroupChatHub>("Hubs/GroupChatHub");
});
//app.MapControllers();

app.Run();
