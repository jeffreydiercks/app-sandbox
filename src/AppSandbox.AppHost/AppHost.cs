var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AppHub>("apphub");

builder.Build().Run();
