using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.EF
{
    public class WebDbContextFactory : IDesignTimeDbContextFactory<WebDbContext>
    {
        public WebDbContext CreateDbContext(string[] args)
        {
            // Đường dẫn tới thư mục chứa appsettings.json
            var appSettingsPath = Path.Combine("D:\\Monweb\\Shopthoitrang\\Web");

            // Kiểm tra file appsettings.json có tồn tại không
            if (!File.Exists(Path.Combine(appSettingsPath, "appsettings.json")))
            {
                throw new FileNotFoundException("Không tìm thấy file appsettings.json", Path.Combine(appSettingsPath, "appsettings.json"));
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(appSettingsPath) // Thiết lập đường dẫn đúng
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<WebDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SQLServerIdentityConnection"));

            return new WebDbContext(optionsBuilder.Options);
        }
    }
}
