using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MirrorSharp.AspNetCore;

namespace SharpLab.Server.AspNetCore {    
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddCors();
        }

        public void ConfigureContainer(ContainerBuilder builder) {
            StartupHelper.ConfigureContainer(builder);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseCors(p => p
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .SetPreflightMaxAge(StartupHelper.CorsPreflightMaxAge)
            );
            
            var okBytes = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes("OK"));
            app.Map("/status", a => a.Use((c, next) => {
                c.Response.ContentType = "text/plain";
                return c.Response.BodyWriter.WriteAsync(okBytes).AsTask();
            }));

            app.UseWebSockets();
            app.UseMirrorSharp(StartupHelper.CreateMirrorSharpOptions(app.ApplicationServices.GetAutofacRoot()));
        }
    }
}
