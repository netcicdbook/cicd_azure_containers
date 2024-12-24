using Components.Communication.MessageBroker.Configuration;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Components.Communication.MessageBroker
{
    public static class ExtendServices
    {
        public static IServiceCollection AddMessageBroker(this IServiceCollection services
                                                             , IConfiguration configuration
                                                             , Assembly? assembly=null)
        {            
            var rabbitMqOptions = new RabbitMqHostOptions();
            configuration.GetSection("MessageBroker").Bind(rabbitMqOptions);

            services.AddMassTransit(busConfiguration => {

                busConfiguration.SetKebabCaseEndpointNameFormatter();
                
                if (assembly != null)
                    busConfiguration.AddConsumers(assembly);

                busConfiguration.UsingRabbitMq((context, rabbitMQconfiguration) => {
                    rabbitMQconfiguration.Host(new Uri(rabbitMqOptions.Host), host =>
                    {
                        host.Username(rabbitMqOptions.UserName);
                        host.Password(rabbitMqOptions.Password);
                    });                    
                    rabbitMQconfiguration.ConfigureEndpoints(context);
                });

            });
            return services;
        }
    }
}
