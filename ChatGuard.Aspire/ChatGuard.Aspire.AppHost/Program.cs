var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ChatGuard_Aspire_ApiService>("ChatGuard")
    .WithEndpoint();

builder.Build().Run();
