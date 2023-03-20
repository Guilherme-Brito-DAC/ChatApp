using ChatApp.Shared;
using ChatApp.Shared.Constantes;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.WebSockets;

namespace ChatApp.Client.Models
{
    public class Conexao
    {
        public HubConnection? conexao;

        public Conexao(Usuario usuario)
        {
            conexao = new HubConnectionBuilder()
                .WithUrl("https://localhost:7287/chathub")
                .Build();

            conexao.StartAsync();

            Mensagem mensagem = new Mensagem()
            {
                UsuarioOrigem = usuario,
                DataHoraEnvio = DateTime.Now,
                Texto = $"{usuario.Nome} conectou"
            };

            EnviarMensagem(TipoMensagem.Conexao, mensagem);
        }

        public async Task EnviarMensagem(TipoMensagem Tipo, Mensagem mensagem)
        {
            if (conexao == null) return;

            await conexao.SendAsync(Tipo.ToString(), mensagem);
        }

        public async ValueTask Desconectar()
        {
            if (conexao == null) return;

            await conexao.DisposeAsync();
        }
    }
}
