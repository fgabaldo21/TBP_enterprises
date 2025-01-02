namespace TBP_enterprises.Models
{
    public class UlogaZaposlenika
    {
        public int zaposlenik_id { get; set; }
        public int uloga_id { get; set; }
        public string Zaposlenik_ime {  get; set; } = string.Empty;
        public string Zaposlenik_prezime { get; set; } = string.Empty;
        public string Uloga_naziv {  get; set; } = string.Empty;
    }
}
