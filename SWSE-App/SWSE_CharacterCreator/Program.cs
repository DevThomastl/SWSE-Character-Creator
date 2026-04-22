using Microsoft.EntityFrameworkCore;
using SWSE_CharacterCreator.Data;
using SWSE_CharacterCreator.Data.Seed;
using SWSE_CharacterCreator.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC and API support
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// database connection
builder.Services.AddDbContext<SagaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// adds ruleset service
builder.Services.AddSingleton<HeroicClassRulesService>();
builder.Services.AddSingleton<SpeciesRulesService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<SagaDbContext>();
    var env = services.GetRequiredService<IWebHostEnvironment>();

    // migrations
    db.Database.Migrate();

    // seed data
    DbSeeder.SeedAll(db, env);
}

// talents and force powers seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<SagaDbContext>();
    var env = services.GetRequiredService<IWebHostEnvironment>();

    await TalentSeeder.SeedAsync(db, env);
    await ForcePowerSeeder.SeedAsync(db, env);
}

// error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();


app.MapControllers();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();