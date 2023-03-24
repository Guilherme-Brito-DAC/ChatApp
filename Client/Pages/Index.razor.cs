using ChatApp.Client.Componentes;
using ChatApp.Client.Models;
using ChatApp.Shared;
using ChatApp.Shared.Constantes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;

namespace ChatApp.Client.Pages
{
    public partial class Index
    {
        #region Parametros
        [Inject]
        NavigationManager? navigationManager { get; set; }

        [Inject]
        Conexao? con { get; set; }

        [Inject]
        Usuario? UsuarioAtual { get; set; }
        #endregion

        public HubConnection? conexao;

        public List<Contato> ContatosOnline = new List<Contato>();

        public List<Usuario> Usuarios = new List<Usuario>();

        public List<Conversa> Conversas = new List<Conversa>();

        public string Id { get; set; }
        public string Mensagem { get; set; }
        public string Pesquisa { get; set; }
        public Usuario UsuarioConversando { get; set; }
        public Conversa ConversaAtual { get; set; }

        #region [Overrides]
        protected override void OnInitialized()
        {
            con?.conexao?.On(TipoMensagem.Conexao.ToString(), (List<Usuario> usuarios) =>
            {
                Usuarios = usuarios;

                AtualizarListaDeContatos(usuarios.Where(u => u.Id != UsuarioAtual.Id).ToList());

                StateHasChanged();
            });

            con?.conexao?.On(TipoMensagem.Desconexao.ToString(), (List<Usuario> usuarios) =>
            {
                Usuarios = usuarios;

                if (Id == UsuarioAtual.Id)
                {
                    Id = "";
                    UsuarioConversando = null;
                }

                if (UsuarioConversando != null && !usuarios.Any(u => u.Id == UsuarioConversando.Id))
                {
                    Id = "";
                    UsuarioConversando = null;
                }

                AtualizarListaDeContatos(usuarios.Where(u => u.Id != UsuarioAtual.Id).ToList());

                StateHasChanged();
            });

            con?.conexao?.On(TipoMensagem.AlteracaoDeUsuario.ToString(), (List<Usuario> usuarios) =>
            {
                Usuarios = usuarios;

                AtualizarListaDeContatos(usuarios.Where(u => u.Id != UsuarioAtual.Id).ToList());

                StateHasChanged();
            });

            con?.conexao?.On(TipoMensagem.UsuariosOnline.ToString(), (List<Usuario> usuarios) =>
            {
                Usuarios = usuarios;

                AtualizarListaDeContatos(usuarios.Where(u => u.Id != UsuarioAtual.Id).ToList());

                StateHasChanged();
            });

            con?.conexao?.On(TipoMensagem.ConversasOnline.ToString(), (List<Conversa> conversas) =>
            {
                Conversas = conversas;

                StateHasChanged();
            });

            con?.conexao?.On(TipoMensagem.Mensagem.ToString(), (Conversa conversa, Mensagem mensagem) =>
            {
                if (UsuarioConversando != null && conversa.usuarios.Any(u => u.Id == UsuarioConversando.Id))
                {
                    ConversaAtual = conversa;
                }

                AtualizarConversa(conversa, mensagem);

                StateHasChanged();
            });

            con?.conexao?.On(TipoMensagem.GetConnectionId.ToString(), (string ConnectionId) =>
            {
                UsuarioAtual.ConnectionID = ConnectionId;
            });

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool primeiroRender)
        {
            if (primeiroRender)
            {
                await con?.EnviarMensagem(TipoMensagem.UsuariosOnline, null);
                await con?.EnviarMensagem(TipoMensagem.ConversasOnline, null);
            }
        }
        #endregion

        public void SelecionarConversa(string _Id)
        {
            Id = _Id;

            UsuarioConversando = Usuarios.First(u => u.Id == _Id);

            Conversa conversa = Conversas.FirstOrDefault(c => c.usuarios.Any(u => u.Id == UsuarioConversando.Id));

            if (conversa != null)
            {
                ConversaAtual = conversa;
            }
            else
            {
                ConversaAtual = null;
            }

            AtualizarNotificacoes(UsuarioConversando);

            StateHasChanged();
        }

        public async Task EnviarMensagem()
        {
            if (string.IsNullOrEmpty(Mensagem))
                return;

            Mensagem mensagem = new Mensagem()
            {
                UsuarioOrigem = UsuarioAtual,
                UsuarioDestino = UsuarioConversando,
                DataHoraEnvio = DateTime.Now,
                Texto = Mensagem,
            };

            await con?.EnviarMensagem(TipoMensagem.Mensagem, mensagem);

            Mensagem = "";
        }

        public void Configuracoes()
        {
            navigationManager?.NavigateTo("perfil");
        }

        #region [Privados]
        private void AtualizarConversa(Conversa conversa, Mensagem mensagem)
        {
            Conversa Conversa = Conversas.FirstOrDefault(c => c.usuarios.Any(l => l.Id == mensagem.UsuarioOrigem.Id) && c.usuarios.Any(l => l.Id == mensagem.UsuarioDestino.Id));

            if (Conversa == null)
            {
                Conversas.Add(conversa);
            }
            else
            {
                int index = Conversas.IndexOf(Conversa);

                if (index != -1)
                    Conversas[index] = conversa;
            }
        }

        private void AtualizarNotificacoes(Usuario usuario)
        {
            Contato Contato = ContatosOnline.Where(u => u.Id == usuario.Id).First();

            int index = ContatosOnline.IndexOf(Contato);

            if (Contato.Selecionado || Id == usuario.Id)
                Contato.Notificacoes = 0;
            else
                Contato.Notificacoes = Contato.Notificacoes + 1;

            if (index != -1)
                ContatosOnline[index] = Contato;

            StateHasChanged();
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
                    Id = usuario.Id,
                    ConnectionId = usuario.ConnectionID,
                    Notificacoes = 0
                });
            }
        }
        #endregion
    }
}
