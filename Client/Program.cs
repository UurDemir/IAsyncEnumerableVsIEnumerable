// See https://aka.ms/new-console-template for more information
using Shared;

using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

string baseURL = "https://localhost:7083/";

Console.WriteLine("IEnumerable vs IAsyncEnumerable");

Stopwatch sw = new();

int counter = 0;

long syncAPITime = 0, asyncAPITime = 0;

if (!Directory.Exists(nameof(CallAPIAsync)))
    Directory.CreateDirectory(nameof(CallAPIAsync));

if (!Directory.Exists(nameof(CallAsyncAPIAsync)))
    Directory.CreateDirectory(nameof(CallAsyncAPIAsync));

while (counter++ < 5)
{
    sw.Restart();
    await CallAPIAsync();
    sw.Stop();
    syncAPITime = sw.ElapsedMilliseconds;

    Console.WriteLine($"Sync API Completed !");
    Console.WriteLine(new string('*', 15));

    sw.Restart();
    await CallAsyncAPIAsync();
    sw.Stop();
    asyncAPITime = sw.ElapsedMilliseconds;
    Console.WriteLine($"Async API Completed !");
    Console.WriteLine(new string('*', 15));


    Console.WriteLine(new string('-', 15));
    Console.WriteLine($"Sync API Time : {syncAPITime}");
    Console.WriteLine($"Async API Time : {asyncAPITime}");
}

Console.ReadKey();

async Task CallAPIAsync()
{
    using HttpClient client = new();

    Console.WriteLine($"[{DateTime.Now:ss:FFFFFF}] Sync Call Started");

    HttpResponseMessage httpResponseMessage = await client.GetAsync($"{baseURL}syncData").ConfigureAwait(false);

    httpResponseMessage.EnsureSuccessStatusCode();
    IEnumerable<DeveloperInformation>? developers = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<DeveloperInformation>>();

    foreach (var developer in developers!)
    {
        Console.WriteLine($"[{DateTime.Now:ss:FFFFFF}]{developer.FullName}");
        string developerContent = JsonSerializer.Serialize(developer);
        File.WriteAllText($"{nameof(CallAPIAsync)}/{DateTime.Now.Ticks}.json", developerContent);
    }
}

async Task CallAsyncAPIAsync()
{
    using HttpClient client = new();

    Console.WriteLine($"[{DateTime.Now:ss:FFFFFF}] Async Call Started");

    Stream stream = await client.GetStreamAsync($"{baseURL}asyncData").ConfigureAwait(false);

    IAsyncEnumerable<DeveloperInformation?> developers = JsonSerializer.DeserializeAsyncEnumerable<DeveloperInformation>(stream, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultBufferSize = 128
    });

    await foreach (var developer in developers)
    {
        Console.WriteLine($"[{DateTime.Now:ss:FFFFFF}]{developer!.FullName}");
        string developerContent = JsonSerializer.Serialize(developer);
        File.WriteAllText($"{nameof(CallAsyncAPIAsync)}/{DateTime.Now.Ticks}.json", developerContent);
    }
}