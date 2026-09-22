using SIA.AdminBff.Clients.Academic;
using SIA.AdminBff.Clients.Workflow;
using SIA.AdminBff.Clients.Scheduling;
using SIA.AdminBff.Configuration;
using SIA.AdminBff.Infrastructure.Http;

namespace SIA.AdminBff.Extensions;

public static class InternalClientsExtensions
{
    public static IServiceCollection AddInternalClients(this IServiceCollection services, IConfiguration configuration)
    {
        var academicServiceBaseAddress = InternalServiceConfiguration.GetRequiredBaseAddress(configuration, InternalServiceConfiguration.AcademicService);
        var schedulingServiceBaseAddress = InternalServiceConfiguration.GetRequiredBaseAddress(configuration, InternalServiceConfiguration.SchedulingService);
        var workflowServiceBaseAddress = InternalServiceConfiguration.GetRequiredBaseAddress(configuration, InternalServiceConfiguration.WorkflowService);

        services.AddTransient<RequestContextPropagationHandler>();

        services.AddHttpClient<IAcademicClient, AcademicClient>(client =>
        {
            client.BaseAddress = academicServiceBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        }).AddHttpMessageHandler<RequestContextPropagationHandler>();

        services.AddHttpClient<ISchedulingClient, SchedulingClient>(client =>
        {
            client.BaseAddress = schedulingServiceBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        }).AddHttpMessageHandler<RequestContextPropagationHandler>();

        services.AddHttpClient<IWorkflowClient, WorkflowClient>(client =>
        {
            client.BaseAddress = workflowServiceBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        }).AddHttpMessageHandler<RequestContextPropagationHandler>();

        return services;
    }
}