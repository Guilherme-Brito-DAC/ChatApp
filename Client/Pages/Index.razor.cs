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
        #region Parametros
        public HubConnection? conexao;
        public List<Contato> ContatosOnline = new List<Contato>();
        public List<Usuario> Usuarios = new List<Usuario>();

        [Inject]
        NavigationManager? navigationManager { get; set; }

        [Inject]
        Conexao? con { get; set; }

        [Inject]
        Usuario? UsuarioAtual { get; set; }
        #endregion

        public string Id { get; set; }
        public string Mensagem { get; set; }
        public Usuario UsuarioConversando { get; set; }

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

            con?.conexao?.On(TipoMensagem.Mensagem.ToString(), (Mensagem mensagem) =>
            {
                if (mensagem.UsuarioDestino.Id == UsuarioAtual.Id)
                {
                    AtualizarNotificacoes(mensagem.UsuarioOrigem);
                }
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
            }
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

        public void SelecionarConversa(string _Id)
        {
            Id = _Id;

            UsuarioConversando = Usuarios.First(u => u.Id == _Id);

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
    }
}
