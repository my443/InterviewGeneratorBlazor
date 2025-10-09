using InterviewGeneratorBlazor.Components;
using InterviewGeneratorBlazor.ViewModels;
using InterviewGeneratorBlazor.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// RegisterViewModels
var connectionString = "Data Source=c:\\temp\\app.db";
builder.Services.AddSingleton(new AppDbContextFactory(connectionString));

builder.Services.AddScoped<CategoryViewModel>();
builder.Services.AddScoped<QuestionViewModel>();
builder.Services.AddTransient<InterviewViewModel>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
