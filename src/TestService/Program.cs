var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async () =>
{

    // Tôi muốn gọi http request ở đây với nội dung curl --location 'http://localhost:6001/search?searchTerm=Nasir'

    var client = new HttpClient();
    // client.BaseAddress = new Uri("http://auction-svc");
    client.BaseAddress = new Uri("http://gateway-svc");
    var request = new HttpRequestMessage(HttpMethod.Get, "/search?searchTerm=Nasir");
    var response = await client.SendAsync(request);
    // Đảm bảo request thành công
    if (response.IsSuccessStatusCode)
    {
        // Đọc nội dung của response
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Response: " + responseContent);
    }
    else
    {
        Console.WriteLine("Error: " + response.StatusCode);
    }
    return "Hello World! 123 456";
});

app.Run();
