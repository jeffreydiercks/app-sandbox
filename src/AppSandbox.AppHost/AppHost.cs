var builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIRECOSMOSDB001
var cosmos = builder.AddAzureCosmosDB("cosmos")
                    .RunAsPreviewEmulator(e => e
                        .WithDataExplorer());
#pragma warning restore ASPIRECOSMOSDB001

builder.AddProject<Projects.AppHub>("apphub", launchProfileName: "https");

builder.AddProject<Projects.MyVerses>("myverses", launchProfileName: "https")
       .WithReference(cosmos)
       .WaitFor(cosmos);

builder.Build().Run();
