namespace TBP_enterprises.Models
{
    public class Obracun
    {
        public int Id { get; set; }
        public int Id_zaposlenik { get; set; }
        public string Zaposlenik_ime {  get; set; } = string.Empty;
        public string Zaposlenik_prezime {  get; set; } = string.Empty;
        public int Id_projekt { get; set; }
        public string Projekt_naziv {  get; set; } = string.Empty;
        public DateTime Datum_obracuna {  get; set; } = DateTime.Now;
        public double Ukupni_trosak { get; set; }
    }
}
