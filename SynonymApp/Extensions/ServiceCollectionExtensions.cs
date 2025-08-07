using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using SynonymApp.Background;
using SynonymApp.Controllers;
using SynonymApp.Infrastructure.BackgroundJobs;
using SynonymApp.Infrastructure.Configuration;
using System.Reflection;

namespace SynonymApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServicesAndRepositories(this IServiceCollection services)
        {
            services.AddSingleton<SynonymsPersistenceChannel>();
            services.AddHostedService<SynonymsPersistenceBackgroundService>();

            return services;
        }

        public static IServiceCollection AddCustomMediator(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(p =>
            {
                p.RegisterServicesFromAssemblies(assemblies);
            });

            return services;
        }

        public static void AddCustomSwagger(this IServiceCollection services, string xmlPath, string? title = null, string? description = null)
        {
            services.AddSwaggerGen(opt =>
            {
                opt.IncludeXmlComments(xmlPath);
                opt.CustomSchemaIds(type => type.ToString().Replace("+", "."));
                opt.OperationFilter<SwaggerJsonIgnore>();

                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = title ?? "Tesvolt API - v1",
                    Description = description ?? "Tesvolt APIs",
                    Version = "v1"
                });

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
        public static IServiceCollection AddCustomVersioning(this IServiceCollection services)
        {
            services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1);
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(
                            new UrlSegmentApiVersionReader(),
                            new HeaderApiVersionReader("X-Api-Version"));
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                });

            return services;
        }

        public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
        {
            IEnumerable<IEndpoint> endpoints = app.Services
                .GetRequiredService<IEnumerable<IEndpoint>>();

            IEndpointRouteBuilder builder =
                routeGroupBuilder is null ? app : routeGroupBuilder;

            foreach (IEndpoint endpoint in endpoints)
            {
                endpoint.MapEndpoint(builder);
            }


            return app;
        }

        public static IServiceCollection AddMinApiEndpoints(this IServiceCollection services, Assembly assembly)
        {
            ServiceDescriptor[] serviceDescriptors = assembly
                .DefinedTypes
                .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                               type.IsAssignableTo(typeof(IEndpoint)))
                .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
                .ToArray();

            services.TryAddEnumerable(serviceDescriptors);

            return services;
        }
    }
}
