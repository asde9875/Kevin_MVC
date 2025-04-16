using Kevin.DataAccess.DAO;
using Kevin.DataAccess.Data;
using KevinWeb.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
//Email Extension
using Microsoft.AspNetCore.Identity.UI.Services;
using Kevin.Utility;
using Stripe;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

//Redirect the users to the login or access denied page
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication()
    .AddFacebook(options =>
    {
        options.AppId = "4149884111908799";
        options.AppSecret = "373c7b45d9123786eafc143f47806ce2";
        options.AccessDeniedPath = "/Identity/Account/ExternalLogin"; // Ensure this points to your login page
    })
    .AddGoogle(options =>
    {
        options.ClientId = "271643373560-lh2vu3dslmm26f24qddhuvjoj1ti7h0i.apps.googleusercontent.com"; // Replace with your Google Client ID
        options.ClientSecret = "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"; // Replace with your Google Client Secret
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddRazorPages();

//Email Extension
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Register DAO and Service layers
builder.Services.AddScoped<ICategoryDao, CategoryDao>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IProductDao, ProductDao>();
builder.Services.AddScoped<IProductService, KevinWeb.Services.ProductService>();

builder.Services.AddScoped<ICompanyDao, CompanyDao>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

builder.Services.AddScoped<IShoppingCartDao, ShoppingCartDao>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

builder.Services.AddScoped<IApplicationUserDao, ApplicationUserDao>();
builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey =builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
