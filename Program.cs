using CVAnalyzer.Data;
using CVAnalyzer.Services;
using CVAnalyzer.Helpers; // Make sure this namespace matches SkillClassifier's location
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(); // Required for session support

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register custom services
builder.Services.AddScoped<CvParserService>();
builder.Services.AddScoped<AffindaService>();
builder.Services.AddScoped<SkillClassifier>(); // ✅ Register this so CvParserService can get it
builder.Services.AddHttpClient();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// **Important: UseSession BEFORE UseRouting**
app.UseSession();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
