namespace ChatApp.Shared
{
    public class Usuario
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Imagem { get; set; }

        public Usuario()
        {
            Random rnd = new Random();

            Id = new Guid().ToString();
            Imagem = "../content/pizza.jpg";
            Nome = $"Usuário {rnd.Next(200)}";
        }
    }
}
