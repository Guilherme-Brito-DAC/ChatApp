using ChatApp.Shared;
using ChatApp.Shared.Constantes;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        public static List<Usuario> Usuarios = new List<Usuario>();

        public static List<Conversa> Conversas = new List<Conversa>();

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
            Conversa Conversa = Conversas.FirstOrDefault(c => c.usuarios.Any(l => l.Id == mensagem.UsuarioOrigem.Id) && c.usuarios.Any(l => l.Id == mensagem.UsuarioDestino.Id));

            if (Conversa == null)
            {
                Conversa = new Conversa();

                Conversa.usuarios.Add(mensagem.UsuarioOrigem);
                Conversa.usuarios.Add(mensagem.UsuarioDestino);

                Conversa.mensagems.Add(mensagem);

                Conversas.Add(Conversa);
            }
            else
            {
                Conversa.mensagems.Add(mensagem);

                int index = Conversas.IndexOf(Conversa);

                if (index != -1)
                    Conversas[index] = Conversa;
            }

            await Clients.Clients(mensagem.UsuarioOrigem.ConnectionID, mensagem.UsuarioDestino.ConnectionID).SendAsync(TipoMensagem.Mensagem.ToString(), Conversa, mensagem);
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

        public async Task ConversasOnline(Mensagem mensagem)
        {
            List<Conversa> conversas = Conversas.Where(c => c.usuarios.Any(l => l.ConnectionID == Context.ConnectionId)).ToList();

            await EnviarMensagemParaMesmoUsuario(TipoMensagem.ConversasOnline, conversas);
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
