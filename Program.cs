using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseDefaultFiles(); // ������������� ���� index.html
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true // ��������� ������� ����������� ������
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();