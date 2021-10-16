using System;

namespace AppFirebaseDemo.Engine.Interfaces
{
	public interface IInterfaceBasica
	{
		string Chave { get; set; }

		DateTime DataCriacao { get; set; }

		DateTime DataAtualizacao { get; set; }
	}

	public static class IInterfaceBasica_JSON
	{
		public static readonly string Chave = "k";
		public static readonly string DataCriacao = "dc";
		public static readonly string DataAtualizacao = "da";
	}
}
