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

        public async Task Conectar()
        {
            conexao = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/chathub"))
                .Build();

            conexao.On<string, string>("MensagemRecebida", (usuario, mensagem) =>
            {
                string msg = $"{(string.IsNullOrEmpty(usuario) ? "" : usuario + ": ")} {mensagem}";
                Mensagens += msg + "\n";
                StateHasChanged();
            });

            await conexao.StartAsync();
        }

        public async Task EnviarMensagem()
        {
            if (conexao == null) return;

            await conexao.SendAsync("MensagemEnviada", Usuario, Mensagem);

            Mensagem = String.Empty;
        }

        public async ValueTask Desconectar()
        {
            if (conexao == null) return;

            await conexao.DisposeAsync();
        }
    }
}
