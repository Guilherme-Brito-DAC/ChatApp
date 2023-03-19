using ChatApp.Shared;
using ChatApp.Shared.Constantes;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        #region Overrides
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
        #endregion

        public async Task Conexao(Mensagem mensagem)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, mensagem.UsuarioOrigem.Id);

            await EnviarMensagem(TipoMensagem.Conexao, mensagem);
        }

        public async Task EnviarMensagem(TipoMensagem tipo, Mensagem mensagem)
        {
            await Clients.All.SendAsync(tipo.ToString(), mensagem);
        }
    }
}
