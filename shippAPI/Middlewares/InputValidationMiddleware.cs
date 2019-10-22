using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace shippAPI.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class InputValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public InputValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var param1 = "lat";
            var param2 = "long";
            var existsLat = httpContext.Request.Query.ContainsKey(param1);
            var existsLong = httpContext.Request.Query.ContainsKey(param2);

            //verify if exists all required params
            if (existsLat && existsLong)
            {
                decimal latitude = 0;
                decimal longitude = 0;

                bool canConvertLat = Decimal.TryParse(httpContext.Request.Query[param1], NumberStyles.Currency ,CultureInfo.InvariantCulture.NumberFormat, out latitude);
                bool canConvertLong = Decimal.TryParse(httpContext.Request.Query[param2], NumberStyles.Currency, CultureInfo.InvariantCulture.NumberFormat, out longitude);

                //verify if all params may be converted to decimal
                if (canConvertLat && canConvertLong)
                {
                    //put all params into httpContext so that the controller can use this variables
                    httpContext.Items["latitude"] = latitude;
                    httpContext.Items["longitude"] = longitude;

                    //OK. Go to next middleware
                    await _next(httpContext);
                }
                else
                {
                    //invalid params
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new ClassError(true, "Invalid params")));
                }
            }
            else
            {
                //missing params
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject( new ClassError(true, "Missing params")));
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddlewareClassTemplate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InputValidationMiddleware>();
        }
    }
}
