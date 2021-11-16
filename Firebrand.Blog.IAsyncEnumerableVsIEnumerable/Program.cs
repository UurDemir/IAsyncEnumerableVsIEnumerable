using Shared;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const int delayTime = 500;

DeveloperInformation[] developers = GenerateDevelopers().ToArray();

IEnumerable<DeveloperInformation> GenerateDevelopers()
{
    for (int i = 0; i < 10; i++)
    {
        DeveloperInformation developer = new()
        {
            Age = Faker.RandomNumber.Next(16,65),
            FullName = Faker.Name.FullName(),
            ProjectCount = Faker.RandomNumber.Next(5,100),
            Company = Faker.Company.Name()
        };

        yield return developer;
    }
}

var randomNumbers = Enumerable.Range(0, 10);

app.MapGet("/", () => "IAsyncEnumerable Vs IEnumerable");

app.MapGet("/syncData", GetDevelopers);
app.MapGet("/asyncData", GetDevelopersAsync);

app.Run();


IEnumerable<DeveloperInformation> GetDevelopers()
{
    foreach (var developer in developers)
    {
        Thread.Sleep(delayTime);
        yield return developer;
    }
}


async IAsyncEnumerable<DeveloperInformation> GetDevelopersAsync()
{
    foreach (var developer in developers)
    {
        await Task.Delay(delayTime);
        yield return developer;
    }
}