namespace TBP_enterprises.Models
{
    public class ProjektKlijenta
    {
        public int Id_klijent {  get; set; }
        public string Naziv_klijenta { get; set; } = string.Empty;
        public int Id_projekt { get; set; }
        public string Naziv_projekta {  get; set; } = string.Empty;
    }
}
