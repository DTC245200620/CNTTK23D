var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Du lieu mau trong bo nho
var products = new List<ProductDto>
{
    new(1, "Giao trinh Lap trinh phan tan",
        "Giao trinh chinh thuc", 150000),

    new(2, "Tai lieu RESTful API",
        "Huong dan thiet ke API", 90000),

    new(3, "Bai tap thuc hanh .NET",
        "Bo bai tap kem loi giai", 120000)
};

// GET / : kiem tra may chu dang chay
app.MapGet("/", () => Results.Ok(new
{
    service = "Catalog.Api",
    status = "running"
}));

// GET /api/products : danh sach san pham
app.MapGet("/api/products", () => Results.Ok(products));

// GET /api/products/{id:int} : chi tiet san pham
app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);

    return product is null
        ? Results.NotFound(new ApiError(
            "PRODUCT_NOT_FOUND",
            "Khong tim thay san pham."))
        : Results.Ok(product);
});

app.Run();

// DTO: hinh dang du lieu di qua mang
record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price);

record ApiError(string Code, string Message);
