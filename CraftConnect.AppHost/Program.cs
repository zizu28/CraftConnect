var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgresdb");
var rabbitmq = builder.AddRabbitMQ("rabbitmq");

builder.AddProject<Projects.UserManagement_Presentation>("usermanagement")
	.WithReference(postgres)
	.WaitFor(postgres)
	.WithReference(rabbitmq)
	.WaitFor(rabbitmq);

builder.AddProject<Projects.BookingManagement_Presentation>("bookingmanagement")
	.WithReference(postgres)
	.WaitFor(postgres)
	.WithReference(rabbitmq)
	.WaitFor(rabbitmq);

builder.Build().Run();
