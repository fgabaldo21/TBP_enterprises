namespace TBP_enterprises.Models
{
    public class Zaposlenik
    {
        public int Id { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public DateTime? PocetakZaposlenja { get; set; }
        public DateTime? ZavrsetakZaposlenja { get; set; }
        public decimal Satnica { get; set; }
    }
}
