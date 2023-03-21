using ChatApp.Shared;
using ChatApp.Shared.Constantes;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        public static List<Usuario> Usuarios = new List<Usuario>();

        #region Overrides
        public override async Task OnConnectedAsync()
        {
            await EnviarMensagemParaMesmoUsuario(TipoMensagem.GetConnectionId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Usuarios = Usuarios.Where(u => u.ConnectionID != Context.ConnectionId).ToList();

            await EnviarMensagemParaTodos(TipoMensagem.Desconexao, Usuarios);

            await base.OnDisconnectedAsync(exception);
        }
        #endregion

        public async Task Conexao(Mensagem mensagem)
        {
            mensagem.UsuarioOrigem.ConnectionID = Context.ConnectionId;

            Usuarios.Add(mensagem.UsuarioOrigem);

            await EnviarMensagemParaTodos(TipoMensagem.Conexao, Usuarios);
        }

        public async Task Mensagem(Mensagem mensagem)
        {
            await Clients.Clients(mensagem.UsuarioOrigem.ConnectionID, mensagem.UsuarioDestino.ConnectionID).SendAsync(TipoMensagem.Mensagem.ToString(), mensagem);
        }

        public async Task EnviarMensagemParaTodos(TipoMensagem tipo, object mensagem)
        {
            await Clients.All.SendAsync(tipo.ToString(), mensagem);
        }

        public async Task EnviarMensagemParaMesmoUsuario(TipoMensagem tipo, object mensagem)
        {
            await Clients.Client(Context.ConnectionId).SendAsync(tipo.ToString(), mensagem);
        }

        public async Task UsuariosOnline(Mensagem mensagem)
        {
            await EnviarMensagemParaMesmoUsuario(TipoMensagem.UsuariosOnline, Usuarios);
        }

        public async Task AlteracaoDeUsuario(Mensagem mensagem)
        {
            Usuario Usuario = Usuarios.Where(u => u.Id == mensagem.UsuarioOrigem.Id).First();

            Usuario.Nome = mensagem.UsuarioOrigem.Nome;
            Usuario.Imagem = mensagem.UsuarioOrigem.Imagem;

            int index = Usuarios.IndexOf(Usuario);

            if (index != -1)
                Usuarios[index] = Usuario;

            await EnviarMensagemParaTodos(TipoMensagem.AlteracaoDeUsuario, Usuarios);
        }
    }
}
