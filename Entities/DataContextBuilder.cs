using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Entities
{
    public class DataContextBuilder : IDbContextBuilder
    {
        public DataContextBuilder(IOptions<Settings> options)
        {
            Settings = options.Value;
        }

        protected Settings Settings { get; }

        public DbContextOptions GetContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataGoogleAPIContext>();
                optionsBuilder.UseNpgsql(Settings.ConnectionString);
            return optionsBuilder.Options;
        }
    }
}
