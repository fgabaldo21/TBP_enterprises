using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using TBP_enterprises.Models;

namespace TBP_enterprises.Pages
{
    public class UlogeZaposlenikaModel : PageModel
    {
        public List<UlogaZaposlenika> UlogeZaposlenika { get; set; } = new List<UlogaZaposlenika>();
        public int TrenutnaStranica { get; set; }
        public int UkupnoStranica { get; set; }

        public void OnGet(int stranica = 1)
        {
            int VelicinaStranice = 10;
            TrenutnaStranica = stranica;

            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var countCommand = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM Uloge_zaposlenika", connection);
                int ukupno = (int)(long)countCommand.ExecuteScalar();
                UkupnoStranica = (int)System.Math.Ceiling((double)ukupno / VelicinaStranice);

                int offset = (TrenutnaStranica - 1) * VelicinaStranice;

                var command = new NpgsqlCommand(
                    "SELECT z.ime, z.prezime, u.naziv, uz.zaposlenik, uz.uloga " +
                    "FROM Uloge_zaposlenika uz " +
                    "JOIN Zaposlenici z ON uz.zaposlenik = z.id_zaposlenik " +
                    "JOIN Uloge u ON uz.uloga = u.id_uloga " +
                    "ORDER BY uz.zaposlenik, uz.uloga LIMIT @Limit OFFSET @Offset",
                    connection);

                command.Parameters.AddWithValue("@Limit", VelicinaStranice);
                command.Parameters.AddWithValue("@Offset", offset);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UlogeZaposlenika.Add(new UlogaZaposlenika
                        {
                            Zaposlenik_ime = reader.GetString(0),
                            Zaposlenik_prezime = reader.GetString(1),
                            Uloga_naziv = reader.GetString(2)
                        });
                    }
                }
            }
        }
    }
}
