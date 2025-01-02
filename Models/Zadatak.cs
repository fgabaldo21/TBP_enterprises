namespace TBP_enterprises.Models
{
    public class Zadatak
    {
        public int Id { get; set; }
        public int Id_projekt { get; set; }
        public string Naziv_projekta { get; set; } = string.Empty;
        public string Naziv_zadatka {  get; set; } = string.Empty;
        public decimal Predvidjeni_sati { get; set; }
        public int Id_status { get; set; }
        public string Naziv_statusa { get; set ; } = string.Empty;
    }
}
