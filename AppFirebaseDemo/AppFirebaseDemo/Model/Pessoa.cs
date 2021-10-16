using AppFirebaseDemo.Engine.Interfaces;
using AppFirebaseDemo.Engine.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppFirebaseDemo.Model
{
	public class Pessoa : BaseModel, IInterfaceBasica
	{
		#region Campos padrão firebase

		[JsonProperty("k")]
		public string Chave { get; set; }

		[JsonProperty("dc")]
		public DateTime DataCriacao { get; set; }

		[JsonProperty("da")]
		public DateTime DataAtualizacao { get; set; }

		#endregion

		public Pessoa(string nome, int idade)
		{
			Nome = nome;
			Idade = idade;
		}

		[JsonProperty("nome")]
		public string Nome { get; set; }

		[JsonProperty("idade")]
		public int Idade { get; set; }

	}
}
