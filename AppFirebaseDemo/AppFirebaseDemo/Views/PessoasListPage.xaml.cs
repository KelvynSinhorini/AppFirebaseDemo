using AppFirebaseDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFirebaseDemo.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PessoasListPage : ContentPage
	{
		private readonly PessoaViewModel Contexto;

		public PessoasListPage()
		{
			InitializeComponent();

			BindingContext = Contexto = new PessoaViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			Contexto.CarregarDados();
		}
	}
}