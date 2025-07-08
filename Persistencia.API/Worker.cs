using Compartilhado;
using Dapper;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Persistencia.API;

public class Worker : BackgroundService
{
	private readonly IConfiguration _configuracao;
	private readonly ConfiguracaoBanco _banco;

	public Worker(IConfiguration configuracao, ConfiguracaoBanco banco)
	{
		_configuracao = configuracao;
		_banco = banco;
	}

	protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
	{
		Console.WriteLine("Iniciando Worker...");

		var factory = new ConnectionFactory()
		{
			HostName = _configuracao["RabbitMQ:Host"],
			UserName = "guest",
			Password = "guest",
		};

		var conexao = await factory.CreateConnectionAsync();
		var canal = await conexao.CreateChannelAsync();

		await canal.QueueDeclareAsync(
			queue: _configuracao["RabbitMQ:Fila"], 
			durable: true, 
			exclusive: false, 
			autoDelete: false
			);

		var consumidor = new AsyncEventingBasicConsumer(canal);

		consumidor.ReceivedAsync += async (_, ea) =>
		{
			var corpo = ea.Body.ToArray();
			var mensagem = Encoding.UTF8.GetString(corpo);
			var contato = JsonSerializer.Deserialize<Contato>(mensagem);

			using var conexaoBanco = _banco.ObtenhaConexao();
			await conexaoBanco.ExecuteAsync(
				"INSERT INTO Contatos (Id, Nome, Ddd, Telefone, Email) " +
				"VALUES (@Id, @Nome, @Ddd, @Telefone, @Email)", 
				contato);

			Console.WriteLine("Mensagem recebida: " + mensagem);
		};

		await canal.BasicConsumeAsync(queue: _configuracao["RabbitMQ:Fila"], autoAck: false, consumer: consumidor);
		return Task.CompletedTask;
	}
}
