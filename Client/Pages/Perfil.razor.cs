using ChatApp.Client.Models;
using ChatApp.Shared;
using Microsoft.AspNetCore.Components;

namespace ChatApp.Client.Pages
{
    public partial class Perfil
    {
        [Inject]
        public Usuario Usuario { get; set; }

        [Inject]
        public Conexao Conexao { get; set; }

        protected string Nome { get; set; }
        protected string Imagem { get; set; }

        protected override void OnInitialized()
        {
            Nome = Usuario.Nome;
            Imagem = Usuario.Imagem;

            base.OnInitialized();
        }

        public void Editar()
        {
            Usuario.Nome = Nome;
            Usuario.Imagem = Imagem;
        }
    }
}
