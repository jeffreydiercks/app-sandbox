var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos")
                    .RunAsEmulator(e => e
                        .WithLifetime(ContainerLifetime.Persistent)
                        .WithDataVolume()
                        .WithUrls(context =>
                        {
                            var emulatorUrl = context.Urls.FirstOrDefault(u => u.Endpoint?.EndpointName == "emulator");
                            if (emulatorUrl != null && Uri.TryCreate(emulatorUrl.Url, UriKind.Absolute, out var uri))
                            {
                                emulatorUrl.Url = $"https://{uri.Host}:{uri.Port}/_explorer/index.html";
                                emulatorUrl.DisplayText = "Data Explorer";
                            }
                        }));

builder.AddProject<Projects.AppHub>("apphub", launchProfileName: "https");

builder.AddProject<Projects.MyGuitar>("myguitar", launchProfileName: "https")
       .WithReference(cosmos)
       .WaitFor(cosmos);

builder.AddProject<Projects.MyLists>("mylists", launchProfileName: "https")
       .WithReference(cosmos)
       .WaitFor(cosmos);

builder.AddProject<Projects.MyReader>("myreader", launchProfileName: "https")
       .WithReference(cosmos)
       .WaitFor(cosmos);

builder.AddProject<Projects.MyVerses>("myverses", launchProfileName: "https")
       .WithReference(cosmos)
       .WaitFor(cosmos);

builder.AddProject<Projects.MyWorkouts>("myworkouts", launchProfileName: "https")
       .WithReference(cosmos)
       .WaitFor(cosmos);

builder.Build().Run();
