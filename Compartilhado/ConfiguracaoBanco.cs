using Microsoft.Data.SqlClient;
using System.Data;

namespace Compartilhado
{
	public class ConfiguracaoBanco
	{
		private readonly string _conexao;

		public ConfiguracaoBanco(string conexao)
		{
			_conexao = conexao;
		}

		public IDbConnection ObtenhaConexao()
		{
			return new SqlConnection(_conexao);
		}
	}
}
