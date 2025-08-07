var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SynonymApp>("synonymapp");

builder.Build().Run();
