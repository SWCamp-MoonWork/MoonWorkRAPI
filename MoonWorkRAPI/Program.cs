using Microsoft.AspNetCore.Mvc.Formatters;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Repository;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IHostRepository, HostRepository>();
builder.Services.AddScoped<IRunRepository, RunRepository>();
builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(jsonOptions =>
    {
        // Respect Properties Casing on Schemas #383
        // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/383
        // Swagger의 기본 케이스인 카멜 케이스를 적용하지 않고, 컨택스트 모델의 케이스를 그대로 유지하도록 하기 위한 조치.
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
        jsonOptions.JsonSerializerOptions.IgnoreNullValues = true;
    });
//builder.Services.AddMvc()
//    .AddMvcOptions(options =>
//    {
//        options.OutputFormatters.Add(new MoonWorkRAPI.PascalCaseJsonProfileFormatter());
//    });


builder.WebHost.ConfigureKestrel(opt =>
{
    opt.ListenAnyIP(5000);
});

var app = builder.Build();
app.UseCors("DocumentationOrigin");
app.UseSwagger();
app.UseSwaggerUI();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
//app.UseRouting();

app.UseAuthorization();

//app.MapRazorPages();

app.Run();