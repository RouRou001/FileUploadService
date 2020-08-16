using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FileUploadService.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using tusdotnet;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Stores;

namespace FileUploadService
{
    public class Startup
    {
        //readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options =>
                    options.AllowAnyOrigin()
                    .WithHeaders(HeaderNames.AccessControlRequestHeaders, "requestverificationtoken")
                );
            });

            services.Configure<FormOptions>(options =>
            {
                // Set the limit to 256 MB
                options.MultipartBodyLengthLimit = 268435456;
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseCors(options => options.WithOrigins("https://localhost:5001", "http://localhost:8100", "https://192.168.0.175:8100"));

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .WithExposedHeaders(tusdotnet.Helpers.CorsHelper.GetExposedHeaders())
                .WithOrigins("https://localhost:5001", "http://localhost:8100", "https://192.168.0.175:8100")
            );

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseTus(httpContext => new DefaultTusConfiguration
            {
                // c:\tusfiles is where to store files
                Store = new TusDiskStore(@"C:\Users\chunc\Documents\RuneLaboratory\Freelance\FileUploadService\StoredFiles\"),
                // On what url should we listen for uploads?
                UrlPath = "/tusUpload",
                Events = new Events
                {
                    OnAuthorizeAsync = eventContext =>
                    {
                        if (!eventContext.HttpContext.User.Identity.IsAuthenticated)
                        {
                            System.Console.WriteLine("DEBUG XXX 2");
                            eventContext.FailRequest(HttpStatusCode.Unauthorized);
                            return Task.CompletedTask;
                        }

                        // Do other verification on the user; claims, roles, etc. In this case, check the username.
                        if (eventContext.HttpContext.User.Identity.Name != "test")
                        {
                            eventContext.FailRequest(HttpStatusCode.Forbidden, "'test' is the only allowed user");
                            return Task.CompletedTask;
                        }

                        // Verify different things depending on the intent of the request.
                        // E.g.:
                        //   Does the file about to be written belong to this user?
                        //   Is the current user allowed to create new files or have they reached their quota?
                        //   etc etc
                        switch (eventContext.Intent)
                        {
                            case IntentType.CreateFile:
                                System.Console.WriteLine("This is Create File API");
                                break;
                            case IntentType.ConcatenateFiles:
                                System.Console.WriteLine("This is Concatenate Files API");
                                break;
                            case IntentType.WriteFile:
                                System.Console.WriteLine("This is Write Files API");
                                break;
                            case IntentType.DeleteFile:
                                System.Console.WriteLine("This is Delete File API");
                                break;
                            case IntentType.GetFileInfo:
                                System.Console.WriteLine("This is Get File Info API");
                                break;
                            case IntentType.GetOptions:
                                System.Console.WriteLine("This is Get Options Files API");
                                break;
                            default:
                                break;
                        }

                        return Task.CompletedTask;
                    },
                    OnFileCompleteAsync = async eventContext =>
                    {
                        // eventContext.FileId is the id of the file that was uploaded.
                        // eventContext.Store is the data store that was used (in this case an instance of the TusDiskStore)

                        // A normal use case here would be to read the file and do some processing on it.
                        ITusFile file = await eventContext.GetFileAsync();
                        var result = await DoSomeProcessing(file, eventContext.CancellationToken);
                        System.Console.WriteLine("Tus File upload complete YEah");

                        async Task<string> DoSomeProcessing(ITusFile file, CancellationToken eventContext)
                        {
                            System.Console.WriteLine("Tus File upload complete YEah");
                            return ("success");
                        }

                        if (result != "success")
                        {
                            //throw new MyProcessingException("Something went wrong during processing");
                        }
                    }
                }
            });
        }
    }
}
