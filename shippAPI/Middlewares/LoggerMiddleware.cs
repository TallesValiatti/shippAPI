using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace shippAPI.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public LoggerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<LoggerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            var strlogs = await FormatRequest(context.Request);
            _logger.LogInformation(strlogs);
            await writeOnFile(strlogs, _logger);

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                strlogs = await FormatResponse(context.Response);
                _logger.LogInformation(strlogs);
                await responseBody.CopyToAsync(originalBodyStream);
                await writeOnFile(strlogs, _logger);
            }
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            var status = response.StatusCode;
            response.Body.Seek(0, SeekOrigin.Begin);

            return $" ****** response ******* {Environment.NewLine}" +
                   $"RESPONSE: {text} {Environment.NewLine}" +
                   $"STATUS: {status}{Environment.NewLine}{Environment.NewLine}" ;
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            request.Body = body;

            return $" ****** request ******* {Environment.NewLine}" +
                   $"PATH: {request.Path} \n" +
                   $"DATE: {DateTime.Now.ToString("dd/MM/yyyy - HH:mm")} {Environment.NewLine}" +
                   $"QUERY PARAMS: {request.QueryString}";
        }

        private async Task writeOnFile(string str, ILogger _logger)
        {
            try
            {
                using (StreamWriter writetext = File.AppendText("log.txt"))
                {
                    writetext.WriteLine(str);
                }
            }
            catch
            {
                _logger.LogError("Error at write on log file");
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddlewareClassTemplate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggerMiddleware>();
        }
    }

    
}
