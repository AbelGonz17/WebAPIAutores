using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;

// forma de documentar de forma global los status code de la api
[assembly:ApiConventionType(typeof(DefaultApiConventions))]
namespace WebApiAutores
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {

            // con esto estamos limpiando el mapeo automatico que se le hace a los typed.claims
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            

            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
                opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
            }).AddJsonOptions(x => x.JsonSerializerOptions
            .ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(opciones =>
                opciones.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //este servicio se encarga de validar los tokens
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                ClockSkew = TimeSpan.Zero
                

                });


            services.AddEndpointsApiExplorer();

            //aqui configuramos para poderle pasar el token a swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "WebAPIAutores", 
                    Version = "v1",
                    Description = "este es un webApi para trabajar con Autores y Libros",
                    Contact = new OpenApiContact
                    {
                        Email = "abelgonzalez@hotmailcom",
                        Name = "Abel Gonzalez",
                        Url = new Uri("https://Gonzalez.blog")
                    },
                    License = new OpenApiLicense
                    {
                        Name ="MIT"
                    },                  
                });

                c.SwaggerDoc("v2", new OpenApiInfo { 
                    Title = "WebAPIAutores", 
                    Version = "v2" });
                c.OperationFilter<AgregarParemetrosHATEOAS>();
                c.OperationFilter<AgregarParemetrosXversion>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaXml = Path.Combine(AppContext.BaseDirectory, archivoXML); 
                c.IncludeXmlComments(rutaXml);
            });

            services.AddAutoMapper(typeof(Startup));     

            //servicio de identity , que es una solución para gestionar usuarios, roles, autenticación y autorización.
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //con esto configuramos para que el authorization sea para el admin
            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("esAdmin", politaca => politaca.RequireClaim("esAdmin"));
                
            });

            services.AddDataProtection();
            services.AddTransient<HashService>();

            //agregando la politica de cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "CantidadTotalRegistros" });
                });
            });

            services.AddTransient<GeneradoEnlaces>();
            services.AddTransient<HATEOSAutorFilterAttribute>();
            services.AddTransient<HATEOASLibroFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger<Startup> logger)
        {
            app.UseLoggeRespuestaHttp();
                   
            if (env.IsDevelopment())
            {
               
            }
            app.UseSwagger();
            app.UseSwaggerUI( c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "webAPIAutores v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "webAPIAutores v2");

            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();
         
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
