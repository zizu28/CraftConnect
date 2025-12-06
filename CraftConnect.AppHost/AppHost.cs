using Core.SharedKernel.ValueObjects;

var builder = DistributedApplication.CreateBuilder(args);

var userDb = builder.AddSqlServer("sql").AddDatabase("CraftConnectDB");
var userModule = builder.AddProject<Projects.UserManagement_Presentation>("userManagement")
	.WithReference(userDb)
	.WaitFor(userDb);

var bookingModule = builder.AddProject<Projects.BookingManagement_Presentation>("bookingmanagement")
	.WithReference(userDb)
	.WaitFor(userModule)
	.WithReference(userModule)
	.WaitFor(userModule);

var ocelotGateway = builder.AddProject < Projects.Core_APIGateway> ("gateway")
	.WithReference(userModule)
	.WaitFor(userModule)
	.WithReference(bookingModule)
	.WaitFor(bookingModule);

var bff = builder.AddProject<Projects.Core_BFF>("bff")
	.WithReference(ocelotGateway)
	.WaitFor(ocelotGateway);

var blazor = builder.AddProject<Projects.CraftConnect_WASM>("blazor")
	.WithReference(bff);

builder.Build().Run();
