using MilligramServer.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace MilligramServer.Database.Helpers;

public static class ApplicationContextHelper
{
    public static DbContextOptions<ApplicationContext> BuildOptions(string connectionString)
    {
        return new DbContextOptionsBuilder<ApplicationContext>().UseSqlServer(connectionString).Options;
    }
}