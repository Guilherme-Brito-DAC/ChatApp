namespace ChatApp.Shared
{
    public class Mensagem
    {
        public Usuario UsuarioOrigem { get; set; }
        public Usuario UsuarioDestino { get; set; }
        public string Texto { get; set; }
        public DateTime DataHoraEnvio { get; set; }
    }
}
