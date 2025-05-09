
using FrontEndDevExtreme.Repository;
using FrontEndDevExtreme.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IWorkItemApiService, WorkItemApiService>();
builder.Services.AddHttpClient<IDepartmentApiService, DepartmentApiService>();
builder.Services.AddHttpClient<IUserApiService, UserApiService>();
builder.Services.AddHttpClient<INoteApiService, NoteApiService>();
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
app.UseCors("AllowAllOrigins");
app.UseAuthorization();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WorkItem}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
