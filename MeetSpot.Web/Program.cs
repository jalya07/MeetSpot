using MeetSpot.Web.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AnalyticsApiService>();
builder.Services.AddScoped<CompanyApiService>();
builder.Services.AddScoped<ReviewApiService>();
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<BookingApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<HeroSlideApiService>();
builder.Services.AddScoped<ContactApiService>();

builder.Services.AddHttpClient("MeetSpotAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5249/");
});

builder.Services.AddScoped<RoomApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
