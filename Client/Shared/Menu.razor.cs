using ChatApp.Client.Models;
using ChatApp.Shared;
using ChatApp.Shared.Constantes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Client.Shared
{
    public partial class Menu
    {
        [Inject]
        Usuario? UsuarioAtual { get; set; }

        [Inject]
        Conexao? con { get; set; }

        protected override void OnInitialized()
        {
            con?.conexao?.On(TipoMensagem.AlteracaoDeUsuario.ToString(), (List<Usuario> usuarios) =>
            {
                StateHasChanged();
            });
        }
    }
}
