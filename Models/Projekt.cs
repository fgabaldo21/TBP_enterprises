namespace TBP_enterprises.Models
{
    public class Projekt
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public DateTime? Datum_pocetka { get; set; }
        public DateTime? Datum_zavrsetka { get; set; }
    }
}
