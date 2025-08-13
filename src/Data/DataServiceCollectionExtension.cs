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

        // Choose 1 of 3 methods for creating the schema
        CreateSchemaFromDbContext(connectionBuilder);
        //CreateSchemaFromGeneratedSql(connectionBuilder);
        //CreateSchemaFromSqlScript(connectionBuilder);

        // Create other database objects
        CreateTriggers(connectionBuilder);

        // Seed database with data
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
    /// Create Schema from Db Context
    /// </summary>
    /// <param name="connectionBuilder"></param>
    private static void CreateSchemaFromDbContext(SqliteConnectionStringBuilder connectionBuilder)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>()
            .UseSqlite(connectionBuilder.ToString(), AddDatabaseOptions);

        var context = new SchoolContext(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }


    /// <summary>
    /// Create Schema from Db Context Generated Sql
    /// </summary>
    /// <param name="connectionBuilder"></param>
    private static void CreateSchemaFromGeneratedSql(SqliteConnectionStringBuilder connectionBuilder)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>()
            .UseSqlite(connectionBuilder.ToString(), AddDatabaseOptions);

        var context = new SchoolContext(optionsBuilder.Options);
        var sql = context.Database.GenerateCreateScript();
        var location = context.Database.GetConnectionString();
        context.Database.ExecuteSqlRaw(sql);
    }


    /// <summary>
    /// Create Schema from Sql Script
    /// </summary>
    /// <param name="connectionBuilder"></param>
    private static void CreateSchemaFromSqlScript(SqliteConnectionStringBuilder connectionBuilder)
    {
        const string RESOURCE_NAME = "ContosoUniversitySchema.sql";
        string resourceFQN = GetResourceFQN(RESOURCE_NAME);
        SqliteConnection conn = new SqliteConnection(connectionBuilder.ToString());
        SqliteCommand command = new SqliteCommand();

        if (conn.State != System.Data.ConnectionState.Open)
            conn.Open();

        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = LoadFromResource(resourceFQN);
        command.Connection = conn;
        command.ExecuteNonQuery();
    }


    /// <summary>
    /// Create Triggers
    /// </summary>
    /// <param name="connectionBuilder"></param>
    private static void CreateTriggers(SqliteConnectionStringBuilder connectionBuilder)
    {
        const string RESOURCE_NAME = "ContosoUniversityTriggers.sql";
        string resourceFQN = GetResourceFQN(RESOURCE_NAME);
        SqliteConnection conn = new SqliteConnection(connectionBuilder.ToString());
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

