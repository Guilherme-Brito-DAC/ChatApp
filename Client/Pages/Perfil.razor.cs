using ChatApp.Client.Models;
using ChatApp.Shared;
using ChatApp.Shared.Constantes;
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

        private int NumeroImagem { get; set; } = 0;

        private string[] Imagens = new string[]
        {
            "pizza",
            "cachorro",
            "coelho",
            "coruja",
            "dino",
            "dragao",
            "panda",
            "pinguim",
            "shiba",
            "unicornio"
        };

        protected override void OnInitialized()
        {
            Nome = Usuario.Nome;
            Imagem = Usuario.Imagem;

            base.OnInitialized();
        }

        public void Anterior()
        {
            if (NumeroImagem > 0)
                NumeroImagem--;
            else
                NumeroImagem = Imagens.Length - 1;

            Imagem = $"../content/{Imagens[NumeroImagem]}.jpg";
            StateHasChanged();
        }

        public void Proxima()
        {
            if (NumeroImagem < Imagens.Length - 1)
                NumeroImagem++;
            else
                NumeroImagem = 0;

            Imagem = $"../content/{Imagens[NumeroImagem]}.jpg";
            StateHasChanged();
        }

        public void Editar()
        {
            Usuario.Nome = Nome;
            Usuario.Imagem = Imagem;

            Mensagem mensagem = new Mensagem()
            {
                UsuarioOrigem = Usuario,
                Texto = $"{Usuario.Nome} alterou seus dados"
            };

            Conexao.EnviarMensagem(TipoMensagem.AlteracaoDeUsuario, mensagem);

            StateHasChanged();
        }
    }
}
