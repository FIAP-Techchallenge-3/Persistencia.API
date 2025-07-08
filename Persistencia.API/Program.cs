using Compartilhado;
using Persistencia.API;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(x =>
	new ConfiguracaoBanco(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
