var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos")
                    .RunAsEmulator(e => e
                        .WithUrlForEndpoint("https", url =>
                        {
                            url.DisplayText = "Data Explorer";
                            url.Url = "/_explorer/index.html";
                        }));

builder.AddProject<Projects.AppHub>("apphub", launchProfileName: "https");

builder.AddProject<Projects.MyVerses>("myverses", launchProfileName: "https")
       .WithReference(cosmos)
       .WaitFor(cosmos);

builder.Build().Run();
