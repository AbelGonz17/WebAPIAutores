using Microsoft.Extensions.Logging;

namespace WebApiAutores.Middlewares
{
    public static class LoggeRespuestaHttpMidlewareExtension
    {
        public static IApplicationBuilder UseLoggeRespuestaHttp(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggeRespuestaHttpMidleware>();
        }
    }

    public class LoggeRespuestaHttpMidleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoggeRespuestaHttpMidleware> logger;

        public LoggeRespuestaHttpMidleware(RequestDelegate siguiente,ILogger<LoggeRespuestaHttpMidleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {                    
                using (var ms = new MemoryStream())
                {
                    var cuerpoOriginalRespuesta = contexto.Response.Body;
                    contexto.Response.Body = ms;

                    await siguiente(contexto);

                    ms.Seek(0, SeekOrigin.Begin);
                    string respuesta = new StreamReader(ms).ReadToEnd();
                    ms.Seek(0, SeekOrigin.Begin);

                    await ms.CopyToAsync(cuerpoOriginalRespuesta);
                    contexto.Response.Body = cuerpoOriginalRespuesta;

                    logger.LogInformation(respuesta);
                }     
        }
    }
}
