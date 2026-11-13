var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// GET / : kiem tra may chu dang chay
app.MapGet("/", () => Results.Ok(new
{
    service = "Catalog.Api",
    status = "running"
}));

app.Run();
