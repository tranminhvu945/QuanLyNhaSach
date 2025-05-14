using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Data;
using System.IO;
using System.Reflection;

namespace QuanLyNhaSach.Configs;

public class DatabaseConfig
{
    private SqliteConnection? _sqliteConnection;
    private DataContext? _dataContext;

    public DataContext DataContext => _dataContext ?? throw new ArgumentNullException("Database not initialized!");

    public static string GetDefaultDatabasePath()
    {
        // Get the directory where the application is running
        string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                              AppDomain.CurrentDomain.BaseDirectory;

        return Path.Combine(appDirectory, "quanlynhasach.db");
    }

    public async Task Initialize(string? dbPath = null)
    {
        try
        {
            dbPath ??= GetDefaultDatabasePath();

            _sqliteConnection = new SqliteConnection($"Data Source={dbPath}");
            await _sqliteConnection.OpenAsync();

            var dbOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(_sqliteConnection)
                .Options;

            _dataContext = new DataContext(dbOptions);
            await _dataContext.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new Exception($"Database initialization failed: {ex.Message}", ex);
        }
    }

    public void CloseConnection()
    {
        if (_sqliteConnection != null)
        {
            _sqliteConnection.Close();
            _sqliteConnection = null;
        }
    }
}