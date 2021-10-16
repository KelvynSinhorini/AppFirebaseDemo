using AppFirebaseDemo.Engine.ViewModel;
using AppFirebaseDemo.Model;
using AppFirebaseDemo.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace AppFirebaseDemo.ViewModels
{
	public class PessoaViewModel : BaseViewModel
	{
		private ObservableCollection<Pessoa> listaPessoa;
		public ObservableCollection<Pessoa> ListaPessoa { get => listaPessoa; set => Set(ref listaPessoa, value); }

		public PessoaViewModel()
		{
			ListaPessoa = new ObservableCollection<Pessoa>();
		}

		public async void CarregarDados()
		{
			try
			{
				PessoaDAO pessoaDAO = DependencyService.Get<PessoaDAO>();

				if (pessoaDAO != null)
				{
					IEnumerable<Pessoa> listaTemp = await pessoaDAO.ListarAtualizacoes(DateTime.MinValue).ConfigureAwait(true);

					bool ocorreuMudanca = false;

					if (listaTemp != null && listaTemp.Any())
					{
						foreach (Pessoa item in listaTemp)
						{
							if (ListaPessoa.FirstOrDefault(x => x.Chave == item.Chave) == null)
							{
								ListaPessoa.Add(item);
								ocorreuMudanca = true;
							}
						}

						if (ocorreuMudanca)
						{
							Changed(() => ListaPessoa);
						}
					}
				}
			}
			catch (Exception ex)
			{
				App.Log(ex.Message);
			}
		}
	}
}
