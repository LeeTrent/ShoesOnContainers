using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductCatalogApi.Data;

namespace ProductCatalogApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CatalogSettings>(Configuration);

            // Console.WriteLine("ConnectionString:");
            // Console.WriteLine(Configuration["ConnectionString"]);
            // Console.WriteLine("-------------------------------------------------");

            // services.AddDbContext<CatalogContext>( 
            //     options => options.UseSqlServer(Configuration["ConnectionString"]) 
            // );

            var server      = Configuration["DatabaseServer"];
            var database    = Configuration["DatabaseName"];
            var user        = Configuration["DatabaseUser"];
            var password    = Configuration["DatabaseUserPassword"];

            // var server      = "192.168.99.100,1433";
            // var database    = "catalogdb";
            // var user        = "sa";
            // var password    = "ProductApi(!)";

            var connectionString = String.Format("Server={0};Database={1};User={2};Password={3};", server, database, user, password);
            Console.WriteLine("connectionString:");
            Console.WriteLine(connectionString);

            services.AddDbContext<CatalogContext>
            (
                    options => options.UseSqlServer(connectionString)
            );

            services.AddMvc();

            services.AddSwaggerGen
            (
                options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc
                    (   "v1", 
                        new Swashbuckle.AspNetCore.Swagger.Info
                        {
                            Title = "ShoesOnContainers - Product Catalog HTTP API",
                            Version = "v1",
                            Description = "The Product Catalog Microservice HTTP API. This is a data-driven (CRUD), microservice example",
                            TermsOfService = "Terms of Service"
                        }
                    );                                
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger().UseSwaggerUI
            (
                c => 
                { 
                    c.SwaggerEndpoint($"/swagger/v1/swagger.json", "ProductCatalogAPI V1");
                }
            );

            app.UseMvc();
        }
    }
}
