using ChatApp.Client.Componentes;
using ChatApp.Client.Models;
using ChatApp.Shared;
using ChatApp.Shared.Constantes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Client.Pages
{
    public partial class Index
    {
        public HubConnection? conexao;
        public string Mensagens = string.Empty;
        public string Usuario = string.Empty;
        public string Mensagem = string.Empty;

        [Inject]
        NavigationManager? navigationManager { get; set; }

        [Inject]
        Conexao? con { get; set; }

        [Inject]
        Usuario? UsuarioAtual { get; set; }

        public List<Contato> ContatosOnline = new List<Contato>()
        {
            new Contato()
            {
                Nome = "João",
                Imagem = "../content/cachorro.jpg",
                Digitando = true,
                UltimaMensagem = "Teste",
                DataHora = DateTime.Now
            }
        };

        protected override void OnInitialized()
        {
            con?.conexao?.On(TipoMensagem.Conexao.ToString(), (Mensagem mensagem) =>
            {
                Console.WriteLine(mensagem.UsuarioOrigem.Id);
                Console.WriteLine(UsuarioAtual?.Id);
                Console.WriteLine(mensagem.UsuarioOrigem.Nome);

                if (mensagem.UsuarioOrigem.Id != UsuarioAtual?.Id && UsuarioAtual?.Id != Guid.Empty.ToString())
                {
                    ContatosOnline.Add(new Contato()
                    {
                        Nome = mensagem.UsuarioOrigem.Nome,
                        Imagem = mensagem.UsuarioOrigem.Imagem,
                        Digitando = false,
                        UltimaMensagem = "Teste",
                        DataHora = mensagem.DataHoraEnvio
                    });

                    StateHasChanged();
                }
            });

            con?.conexao?.On(TipoMensagem.Desconexao.ToString(), (Mensagem mensagem) =>
            {
                if (mensagem.UsuarioOrigem.Id != UsuarioAtual?.Id)
                {
                    ContatosOnline.Add(new Contato()
                    {
                        Nome = mensagem.UsuarioOrigem.Nome,
                        Imagem = mensagem.UsuarioOrigem.Imagem,
                        Digitando = false,
                        UltimaMensagem = "Teste",
                        DataHora = mensagem.DataHoraEnvio
                    });

                    StateHasChanged();
                }
            });

            base.OnInitialized();
        }
    }
}
