using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ContosoUniversity.Data;

public static class DataServiceCollectionExtension
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Caching
        //services.AddMemoryCache();

        // Build either an in-memory or on disk database
        string databaseName = configuration.GetConnectionString("SchoolDataConnection");
        var connectionBuilder = CreateConnectionBuilder(databaseName, SqliteOpenMode.Memory);

        CreateDatabase(connectionBuilder, services);
        SeedSchoolDatabase(connectionBuilder);

        // Database Context(s)
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        services.AddDbContext<SchoolContext>(builder =>
        {
            builder.UseLoggerFactory(loggerFactory);
            builder.UseSqlite(connectionBuilder.ToString(), AddDatabaseOptions);
        },
            ServiceLifetime.Scoped);

        return services;
    }


    private static void CreateDatabase(SqliteConnectionStringBuilder connectionBuilder, IServiceCollection services)
    {
        SqliteConnection connection = new SqliteConnection(connectionBuilder.ToString());

        connection.Open();
        CreateSchema(connection);

        if (connectionBuilder.Mode == SqliteOpenMode.Memory)
        {
            // This connection is used only to keep the In-Memory databases alive throughout the demo.
            services.AddKeyedSingleton<SqliteConnection>("InMemoryReadOnly", connection);
        }
        else
        {
            connection.Close();
        }
    }

    /// <summary>
    /// Build Sqlite Connection string
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// See: https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings
    /// See: https://www.sqlite.org/wal.html
    /// </remarks>
    private static SqliteConnectionStringBuilder CreateConnectionBuilder(string dataSource, SqliteOpenMode mode = SqliteOpenMode.Memory)
    {
        if (mode != SqliteOpenMode.Memory)
            dataSource = BuildPathName(dataSource);

        return new SqliteConnectionStringBuilder()
        {
            Mode = mode,
            DataSource = dataSource,
            Pooling = true,
            DefaultTimeout = 30,
            Cache = SqliteCacheMode.Shared,         // Do NOT use with Write-Ahead Logging
        };
    }

    private static string BuildPathName(string databaseName)
    {
        var dbFileName = String.Format("{0}.db3", databaseName);
        var pathName = Path.Combine(Path.GetTempPath(), dbFileName);

        if (File.Exists(pathName))
            File.Delete(pathName);

        return pathName;
    }


    /// <summary>
    /// Create Schema
    /// </summary>
    /// <param name="conn"></param>
    /// <remarks>
    /// There are 3 ways to create the db schema
    /// The first two use EF code first approach.  However, it does not always produces the desired SQL Scripts
    /// 1. Use the EnsureCreated.  Optionally preface with EnsureDeleted to delete existing database.
    ///    context.Database.EnsureDeleted();
    ///    context.Database.EnsureCreated();
    /// 2. Generate the Sql script and execute it directly
    ///    var sql = context.Database.GenerateCreateScript();
    ///    var location = context.Database.GetConnectionString();
    ///    context.Database.ExecuteSqlRaw(sql);
    /// 3. Read in manually generated Sql script and execute it directly as is below
    /// </remarks>
    private static void CreateSchema(SqliteConnection conn)
    {
        const string RESOURCE_NAME = "ContosoUniversity.sql";
        string resourceFQN = GetResourceFQN(RESOURCE_NAME);
        SqliteCommand command = new SqliteCommand();

        if (conn.State != System.Data.ConnectionState.Open)
            conn.Open();

        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = LoadFromResource(resourceFQN);
        command.Connection = conn;
        command.ExecuteNonQuery();
    }


    private static string GetResourceFQN(string resourceName)
    {
        var assembly = typeof(DataServiceCollectionExtension).Assembly;
        var resourceNames = assembly.GetManifestResourceNames();

        return resourceNames.Where(x => x.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
                            .First();
    }

    private static string LoadFromResource(string resourceFQN)
    {
        string resourceData;
        var assembly = typeof(DataServiceCollectionExtension).Assembly;

        using (Stream stream = assembly.GetManifestResourceStream(resourceFQN))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                resourceData = reader.ReadToEnd();
            }
        }

        return resourceData;
    }

    private static void SeedSchoolDatabase(SqliteConnectionStringBuilder connectionBuilder)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>()
            .UseSqlite(connectionBuilder.ToString(), AddDatabaseOptions);

        var context = new SchoolContext(optionsBuilder.Options);
        SchoolDatabaseSeeder.Seed(context);
    }



    private static void AddDatabaseOptions(SqliteDbContextOptionsBuilder builder)
    {
        builder.CommandTimeout(60)
               .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
    }

}

