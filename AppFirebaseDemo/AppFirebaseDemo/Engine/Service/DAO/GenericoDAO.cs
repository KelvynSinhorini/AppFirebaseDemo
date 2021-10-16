using AppFirebaseDemo.Engine.Interfaces;
using AppFirebaseDemo.Engine.Service.Firebase;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFirebaseDemo.Engine.Service.DAO
{
	public class GenericoDAO<T> where T : class
	{
		#region Variaveis

		protected ChildQuery Child;

		public string NodoFirebase { get; protected set; }

		#endregion

		public GenericoDAO(string nomeNodoFirebase)
		{
			CriarConexao(nomeNodoFirebase);
		}

		protected void CriarConexao(string nomeNodoFirebase)
		{
			NodoFirebase = nomeNodoFirebase;

			Child = null;
			foreach (string nodo in NodoFirebase.Split('.'))
			{
				if (Child == null)
					Child = FirebaseHelper.Client.Child(nodo);
				else
					Child = Child.Child(nodo);
			}
		}

        #region Metodos

        protected async Task<bool> Adicionar(T value, string key = null)
        {
            try
            {
				Console.WriteLine($"GenericoDAO.Adicionar<{nameof(T)}>({((IInterfaceBasica)value)?.Chave}).before");
                
                if (value != null)
                {
                    if ((((IInterfaceBasica)value).DataCriacao == DateTime.MinValue) ||
                        (((IInterfaceBasica)value).DataCriacao == null))
                    {
                        ((IInterfaceBasica)value).DataCriacao = DateTime.UtcNow;
                    }
                    ((IInterfaceBasica)value).DataAtualizacao = ((IInterfaceBasica)value).DataCriacao;

                    if (key == null)
                    {
                        FirebaseObject<T> valueAdd = await Child.PostAsync(value).ConfigureAwait(true);
                        ((IInterfaceBasica)value).Chave = valueAdd.Key;
                        await Child.Child(valueAdd.Key).PutAsync(value).ConfigureAwait(true);
                    }
                    else
                    {
                        ((IInterfaceBasica)value).Chave = key;
                        // await Child.Child(((IInterfaceBasica)value).Chave).PatchAsync(value).ConfigureAwait(true);
                        await Child.Child(((IInterfaceBasica)value).Chave).PatchAsync(value).ConfigureAwait(true);
                    }
                }
                Console.WriteLine("GenericoDAO.Adicionar().after");
                return true;
            }
            catch (Exception ex)
            {
                App.Log(ex.Message);
                return false;
            }
        }

        protected async Task<bool> Atualizar(T value)
        {
            try
            {
                Console.WriteLine($"GenericoDAO.Atualizar<{nameof(T)}>({((IInterfaceBasica)value)?.Chave}).before");
                if (value != null)
                {
                    ((IInterfaceBasica)value).DataAtualizacao = DateTime.UtcNow;

                    await Child.Child(((IInterfaceBasica)value).Chave).PatchAsync(value).ConfigureAwait(true);
                }
                Console.WriteLine("GenericoDAO.Atualizar().after");
                return true;
            }
            catch (Exception ex)
            {
                App.Log(ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<T>> ListarAtualizacoes(DateTime DataAtualizacaoMaisRecente)
        {
            try
            {
                Console.WriteLine($"GenericoDAO.ListarAtualizacoes<{nameof(T)}>(DateTime DataAtualizacaoMaisRecente).before");

                string sDataAtualizacao = DataAtualizacaoMaisRecente.AddMilliseconds(1).ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ");

                IReadOnlyCollection<FirebaseObject<T>> seletor = (await Child.OrderBy(IInterfaceBasica_JSON.DataAtualizacao).StartAt(sDataAtualizacao).OnceAsync<T>().ConfigureAwait(true));

                IEnumerable<T> lista = seletor?.Select(x => x.Object)?.ToList();

                Console.WriteLine($"GenericoDAO.ListarAtualizacoes<{nameof(T)}>(DateTime DataAtualizacaoMaisRecente).after");
                return lista;
            }
            catch (Exception ex)
            {
                App.Log(ex.Message);
                return default;
            }
        }

        #endregion
    }
}
