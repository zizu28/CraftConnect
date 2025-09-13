var builder = DistributedApplication.CreateBuilder(args);

//var postgres = builder.AddPostgres("postgresdb")
//	.WithHostPort(port: 5432)
//	.WithPgAdmin(admin =>
//	{
//		admin.WithHostPort(port: 5050);
//	})
//	.AddDatabase("CraftConnectDb");

var sqlServer = builder.AddSqlServer("sqlserver")
	.WithHostPort(port: 1433)
	.AddDatabase("CraftConnectDb");


var rabbitmq = builder.AddRabbitMQ("rabbitmq");

builder.AddProject<Projects.UserManagement_Presentation>("usermanagement")
	.WithReference(sqlServer)
	.WaitFor(sqlServer)
	.WithReference(rabbitmq)
	.WaitFor(rabbitmq);

builder.AddProject<Projects.BookingManagement_Presentation>("bookingmanagement")
	.WithReference(sqlServer)
	.WaitFor(sqlServer)
	.WithReference(rabbitmq)
	.WaitFor(rabbitmq);

builder.AddProject<Projects.ProductInventoryManagement_Presentation>("productinventorymanagement-presentation");

builder.AddProject<Projects.PaymentManagement_Presentation>("paymentmanagement-presentation");

builder.Build().Run();
