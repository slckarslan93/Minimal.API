using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Minimal.API;

var builder = WebApplication.CreateBuilder(args);

//service registration start

builder.Services.AddScoped<PeopleService>();
builder.Services.AddScoped<GuideGenerator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//service registration end

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//Middleware starts


app.MapGet("get-example", () => "Hello from GET");

app.MapPost("post-example", () => "Hello from POST");

app.MapGet("ok-object", () => Results.Ok(new { Message = "API is working..." }));

app.MapGet("slow-request", async () =>
{
    await Task.Delay(2000);

    return Results.Ok(new
    {
        Message = "Slow API is working..."
    });
});


app.MapGet("get",() => "This is a GET request");
app.MapPost("post",() => "This is a POST request");
app.MapPut("put",() => "This is a PUT request");
app.MapDelete("delete",() => "This is a DELETE request");


app.MapMethods("options-or-head", new[] {"HEAD","OPTIONS" },() => "Hello from either options or head"); //minimal api de olmayan api tiplerini de bu sekilde kullanabiliriz.


var handler = () => "This is coming from a var";


app.MapGet("handler", handler);


app.MapGet("fromClass", () => Example.SomeMethod());

app.MapGet("get-params/{age:int}", (int age) =>
{
    return $"Age provided was {age}";
});

app.MapGet("cars/{carId:regex(^[a-z0-9]+$)}", (string carId) =>
{
    return $"Car is provided was : {carId}";
});

app.MapGet("books/{isbn:length(13)}", (string isbn) =>
{
    return $"Isbn is provided was : {isbn}";
});


app.MapGet("people/search", (string? searchTerm, PeopleService peopleService) =>
{
    if (searchTerm is null) return Results.NotFound();

    var result = peopleService.Search(searchTerm);

    return Results.Ok(result);
});


app.MapGet("mix/{routeParams}", ([FromRoute]string routeParams, [FromQuery(Name = "q")]int queryParams, [FromServices]GuideGenerator guideGenerator) =>
{
    return $"{routeParams} {queryParams} {guideGenerator.NewGuid}";
});


app.MapPost("people", (Person person) =>
{
    return Results.Ok(person);
});


app.MapGet("HttpContext", async context =>
{
    await context.Response.WriteAsync($"Hello from the httpContext");

});

app.MapGet("http", async (HttpRequest request , HttpResponse response) =>
{
    var queryString = request.QueryString;
    await response.WriteAsync($"Query string: {queryString}");
});

app.MapGet("claims", (ClaimsPrincipal user) =>
{
    var claims = user.Claims.ToList();
    return Results.Ok(claims);
});

app.MapGet("cancel", (CancellationToken cancellationToken) =>
{
    return Results.Ok("This is a cancelable request");
});


app.MapGet("get-point", (MapPoint point) =>
{
    return Results.Ok(point);
});

app.MapGet("simple-string", () => "Hello world");
app.MapGet("json-raw-obj", () => new { Message = "Hello world" });
app.MapGet("ok-obj", () => Results.Ok(new { Message = "Hello world"}));
app.MapGet("json-obj", () => Results.Json(new { Message = "Hello world"}));
app.MapGet("text-string", () => Results.Text("Hello world"));
app.MapGet("redirect", () => Results.Redirect("https://google.com"));
app.MapGet("download", () => Results.File(".my/document.txt"));


app.MapGet("logging", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello from endpoint");

    return Results.Ok(logger);
});




//Miiddleware ends

app.Run();
