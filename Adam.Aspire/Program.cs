var builder = DistributedApplication.CreateBuilder(args);

// Apparently aspire isn't supporting functions yet
// builder.AddProject<Projects.Adam_Functions>("Functions")
//     .WithExternalHttpEndpoints();

builder.AddProject<Projects.Adam_WebAPI>("WebAPI")
    .WithExternalHttpEndpoints();

builder.Build().Run();