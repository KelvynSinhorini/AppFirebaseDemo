using AppFirebaseDemo.Service.Firebase;
using ChavesSecretas;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFirebaseDemo
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			Construir();

			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}

		private static void Construir()
		{
			try
			{
				// 1. Inicializa as variaveis estáticas que configuram o banco de dados do Firebase e do Firestore, se esta configuracao nao ocorrer a aplicacao nao conecta no banco dedados
				// 1.1. É preciso lembrar que é preciso configurar corretamente no Firebase e nas propriedades do Projeto do Android
				FirebaseHelper.FirebaseUrl = Chaves.FirebaseURL;
				FirebaseHelper.FirebaseApiKey = Chaves.FirebaseApiKey;
				FirebaseHelper.FirebaseSignInEmail = Chaves.FirebaseSignInEmail;
				FirebaseHelper.FirebaseSignInPassword = Chaves.FirebaseSignInPassword;
				// FireStoreHelper.FireStoreUrl = "";

				// 2. Faz uma requisicao para gerar a primeira autenticacao na carga do aplicativo, este metodo garante que seja gerada a autenticacao inicial
				new Command(async () =>
				{
					await FirebaseHelper.GetAuth().ConfigureAwait(true);
				}).Execute(null);
			}
			catch (Exception ex)
			{
				App.Log(ex.Message);
				// 3. Caso ocorra algo de errado, eh regado uma mensagem de erro dentro do firebase
				// CatalogarErroHelper.Registrar(ex);
			}
		}

		public static void Log(string text,
				[System.Runtime.CompilerServices.CallerFilePath] string file = "",
				[System.Runtime.CompilerServices.CallerMemberName] string member = "",
				[System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
		{
			// Console.WriteLine("**** {0}_{1}({2}): {3}", Path.GetFileName(file), member, line, text);

			Console.WriteLine("****");
			Console.WriteLine($"Mensagem: {text}");
			Console.WriteLine($"Arquivo: {System.IO.Path.GetFileName(file)}:({line})");
			Console.WriteLine($"Metodo: {member}");
			Console.WriteLine("****");
		}
	}
}
