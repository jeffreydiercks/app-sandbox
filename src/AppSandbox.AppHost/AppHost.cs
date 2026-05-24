var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos")
                    .RunAsEmulator();

builder.AddProject<Projects.AppHub>("apphub", launchProfileName: "https");

builder.AddProject<Projects.MyVerses>("myverses", launchProfileName: "https")
       .WithReference(cosmos)
       .WaitFor(cosmos);

builder.Build().Run();
