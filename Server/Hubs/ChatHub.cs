using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await EnviarMensagem("Usuario conectado", "");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await EnviarMensagem("Usuario desconectado", "");

            await base.OnDisconnectedAsync(exception);
        }

        public async Task EnviarMensagem(string mensagem, string usuario)
        {
            await Clients.All.SendAsync("MensagemRecebida", usuario, mensagem);
        }
    }
}
