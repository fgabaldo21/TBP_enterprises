namespace TBP_enterprises.Models
{
    public class RadniSat
    {
        public int Id { get; set; }
        public int Id_zaposlenik { get; set; }
        public string Zaposlenik_ime { get; set; } = string.Empty;
        public string Zaposlenik_prezime {  get; set; } = string.Empty;
        public int Id_zadatak { get; set; }
        public string Naziv_zadatka {  get; set; } = string.Empty;
        public DateTime Datum {  get; set; } = DateTime.Now;
        public int Odradjeni_sati { get; set; }
    }
}
