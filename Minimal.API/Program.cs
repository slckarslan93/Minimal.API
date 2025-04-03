using Minimal.API;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


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

app.Run();
