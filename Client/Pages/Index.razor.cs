using ChatApp.Client.Componentes;
using ChatApp.Client.Models;
using ChatApp.Shared;
using ChatApp.Shared.Constantes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;

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

        public List<Contato> ContatosOnline = new List<Contato>();

        protected override void OnInitialized()
        {
            con?.conexao?.On(TipoMensagem.Conexao.ToString(), (List<Usuario> usuarios) =>
            {
                AtualizarListaDeContatos(usuarios.Where(u => u.Id != UsuarioAtual.Id).ToList());

                StateHasChanged();
            });

            con?.conexao?.On(TipoMensagem.Desconexao.ToString(), (List<Usuario> usuarios) =>
            {
                AtualizarListaDeContatos(usuarios.Where(u => u.Id != UsuarioAtual.Id).ToList());

                StateHasChanged();
            });

            con?.conexao?.On(TipoMensagem.AlteracaoDeUsuario.ToString(), (List<Usuario> usuarios) =>
            {
                AtualizarListaDeContatos(usuarios.Where(u => u.Id != UsuarioAtual.Id).ToList());

                StateHasChanged();
            });

            base.OnInitialized();
        }

        private void AtualizarListaDeContatos(List<Usuario> usuarios)
        {
            ContatosOnline = new List<Contato>();

            foreach (Usuario? usuario in usuarios)
            {
                ContatosOnline.Add(new Contato()
                {
                    Nome = usuario.Nome,
                    Imagem = usuario.Imagem,
                    DataHora = DateTime.Now
                });
            }
        }
    }
}
