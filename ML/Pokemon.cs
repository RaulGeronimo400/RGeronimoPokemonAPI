namespace ML
{
    public class Pokemon
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string URL { get; set; }
        public string Image { get; set; }
        public string Previus { get; set; }
        public string Next { get; set; }
        public List<object> Pokemons { get; set; }
        public Detalles Detalles { get; set; }
    }
}