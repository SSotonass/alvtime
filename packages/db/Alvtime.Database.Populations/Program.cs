using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;

namespace Alvtime.Database.Populations;

class Program
{
    /// <summary>
    /// Documentation of DbUp: https://dbup.readthedocs.io/
    /// </summary>
    static int Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(typeof(Program).Assembly)
            .Build();

        var connectionString = args.FirstOrDefault() ?? configuration["ConnectionString"];

        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            return -1;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ResetColor();
        return 0;
    }
}