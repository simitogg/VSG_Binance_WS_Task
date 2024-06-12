using System.Net.Http.Headers;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static string contentType;

    static async Task Main(string[] args)
    {
        Console.Write("Enter content type (application/json or application/xml): ");
        contentType = Console.ReadLine();

        while (true)
        {
            Console.WriteLine("Enter command (24h or sma) or 'exit' to quit:");
            var command = Console.ReadLine();

            if (command.ToLower() == "exit")
            {
                break;
            }

            switch (command.ToLower())
            {
                case "24h":
                    await Handle24hAvgPrice();
                    break;
                case "sma":
                    await HandleSimpleMovingAverage();
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    break;
            }
        }
    }

    private static async Task Handle24hAvgPrice()
    {
        Console.Write("Enter symbol: ");
        var symbol = Console.ReadLine();

        var requestUri = $"http://localhost:5000/api/{symbol}/24hAvgPrice";
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

        var response = await client.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {result}");
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
        }
    }

    private static async Task HandleSimpleMovingAverage()
    {
        Console.Write("Enter symbol: ");
        var symbol = Console.ReadLine();

        Console.Write("Enter number of data points (n): ");
        var n = Console.ReadLine();

        Console.Write("Enter time period (p): ");
        var p = Console.ReadLine();

        Console.Write("Enter start date (s) (optional, press Enter to skip): ");
        var s = Console.ReadLine();

        var requestUri = $"http://localhost:5000/api/{symbol}/SimpleMovingAverage?n={n}&p={p}";

        if (!string.IsNullOrEmpty(s))
        {
            requestUri += $"&s={s}";
        }

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

        var response = await client.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {result}");
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
        }
    }
}