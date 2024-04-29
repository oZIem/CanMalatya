using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using WebApiii.Models;
using Microsoft.Extensions.Configuration;

namespace WebApiii
{
    public class Startup
    {
       public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProductsContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ProductsConnection"));
            });

            services.AddDbContext<JobPostingsContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("JobPostingsConnection"));
            });


        }



    }
}
