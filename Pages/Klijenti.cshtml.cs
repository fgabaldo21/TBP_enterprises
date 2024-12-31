using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;

namespace TBP_enterprises.Pages
{
    public class KlijentiModel : PageModel
    {
        public List<Klijent> Klijenti { get; set; } = new List<Klijent>();
        public int TrenutnaStranica { get; set; }
        public int UkupnoStranica { get; set; }
        public void OnGet(int stranica = 1)
        {
            const int VelicinaStranice = 10;
            TrenutnaStranica = stranica;

            Klijenti = new List<Klijent>();

            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();

                var countCommand = new Npgsql.NpgsqlCommand("SELECT COUNT(*) FROM Klijenti", connection);
                int ukupno = (int)(long)countCommand.ExecuteScalar();
                UkupnoStranica = (int)Math.Ceiling((double)ukupno / VelicinaStranice);

                var command = new Npgsql.NpgsqlCommand(
                    "SELECT * FROM Klijenti ORDER BY id_klijent LIMIT @VelicinaStranice OFFSET @Offset",
                    connection);
                command.Parameters.AddWithValue("@VelicinaStranice", VelicinaStranice);
                command.Parameters.AddWithValue("@Offset", (stranica - 1) * VelicinaStranice);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Klijenti.Add(new Klijent
                        {
                            Id = reader.GetInt32(0),
                            Naziv_klijenta = reader.GetString(1),
                            Kontakt_email = reader.GetString(2)
                        });
                    }
                }
            }
        }
    }
}
