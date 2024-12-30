using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;

namespace TBP_enterprises.Pages
{
    public class ProjektiModel : PageModel
    {
        public List<Projekt> Projekti { get; set; } = new List<Projekt>();
        public int TrenutnaStranica { get; set; }
        public int UkupnoStranica { get; set; }

        public void OnGet(int stranica = 1)
        {
            const int VelicinaStranice = 10;
            TrenutnaStranica = stranica;

            Projekti = new List<Projekt>();

            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();

                var countCommand = new Npgsql.NpgsqlCommand("SELECT COUNT(*) FROM Projekti", connection);
                int ukupnoZaposlenika = (int)(long)countCommand.ExecuteScalar();
                UkupnoStranica = (int)Math.Ceiling((double)ukupnoZaposlenika / VelicinaStranice);

                var command = new Npgsql.NpgsqlCommand(
                    "SELECT * FROM Projekti ORDER BY id_projekt LIMIT @VelicinaStranice OFFSET @Offset",
                    connection);
                command.Parameters.AddWithValue("@VelicinaStranice", VelicinaStranice);
                command.Parameters.AddWithValue("@Offset", (stranica - 1) * VelicinaStranice);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Projekti.Add(new Projekt
                        {
                            Id = reader.GetInt32(0),
                            Naziv = reader.GetString(1),
                            Datum_pocetka = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                            Datum_zavrsetka = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
                        });
                    }
                }
            }
        }
    }
}
