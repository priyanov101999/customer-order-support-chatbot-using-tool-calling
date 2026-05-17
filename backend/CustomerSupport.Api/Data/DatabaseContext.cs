using System.Data;
using MySqlConnector;

namespace CustomerSupport.Api.Data;

public class DatabaseContext
{
    private readonly IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(
            _configuration.GetConnectionString("DefaultConnection")
        );
    }
}