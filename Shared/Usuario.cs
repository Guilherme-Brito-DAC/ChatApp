namespace ChatApp.Shared
{
    public class Usuario
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Imagem { get; set; }
        public string ConnectionID { get; set; }

        public Usuario()
        {
            Random rnd = new Random();

            Id = Guid.NewGuid().ToString();
            Imagem = "../content/pizza.jpg";
            Nome = $"Usuário {rnd.Next(200)}";
        }
    }
}
