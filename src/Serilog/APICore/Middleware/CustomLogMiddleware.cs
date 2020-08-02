using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Middleware
{
    public class CustomLogMiddleware : IMiddleware
    {

        public string BaseName { get; set; }
        public string Connection { get; set; }
        public CustomLogMiddleware(string baseName, string connection)
        {
            BaseName = baseName;
            Connection = connection;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {


            await next(context);
        }
    }

    public static class CustomLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomLogMiddleware(
            this IApplicationBuilder builder, Action<CustomLogMiddleware> action)
        {
           
            return builder.UseMiddleware<CustomLogMiddleware>();
        }
    }
 }
