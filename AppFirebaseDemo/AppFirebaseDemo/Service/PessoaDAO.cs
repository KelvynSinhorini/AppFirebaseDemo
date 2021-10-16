using AppFirebaseDemo.Engine.Service.DAO;
using AppFirebaseDemo.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppFirebaseDemo.Service
{
	public class PessoaDAO : GenericoDAO<Pessoa>
	{
		public PessoaDAO() : base("app.dev.pessoas")
		{
		}
	}
}
