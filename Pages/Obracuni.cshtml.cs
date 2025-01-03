using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TBP_enterprises.Models;
using Npgsql;

namespace TBP_enterprises.Pages
{
    public class ObracuniModel : PageModel
    {
        public List<Obracun> Obracuni { get; set; } = new List<Obracun>();
        public int TrenutnaStranica { get; set; }
        public int UkupnoStranica { get; set; }

        public void OnGet(int stranica = 1)
        {
            const int VelicinaStranice = 10;
            TrenutnaStranica = stranica;

            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var countCommand = new NpgsqlCommand("SELECT COUNT(*) FROM Zadaci", connection);
                int ukupno = (int)(long)countCommand.ExecuteScalar();
                UkupnoStranica = (int)Math.Ceiling((double)ukupno / VelicinaStranice);

                var command = new NpgsqlCommand(
                    @"SELECT 
                        o.id_obracun, 
                        o.zaposlenik, 
                        z.ime AS zaposlenik_ime, 
                        z.prezime AS zaposlenik_prezime, 
                        o.projekt, 
                        p.naziv_projekta AS projekt_naziv, 
                        o.datum_obracuna, 
                        o.ukupni_trosak
                      FROM Obracuni o
                      JOIN Zaposlenici z ON o.zaposlenik = z.id_zaposlenik
                      JOIN Projekti p ON o.projekt = p.id_projekt
                      ORDER BY o.id_obracun
                      LIMIT @VelicinaStranice OFFSET @Offset",
                    connection);

                command.Parameters.AddWithValue("@VelicinaStranice", VelicinaStranice);
                command.Parameters.AddWithValue("@Offset", (stranica - 1) * VelicinaStranice);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Obracuni.Add(new Obracun
                        {
                            Id = reader.GetInt32(0),
                            Id_zaposlenik = reader.GetInt32(1),
                            Zaposlenik_ime = reader.GetString(2),
                            Zaposlenik_prezime = reader.GetString(3),
                            Id_projekt = reader.GetInt32(4),
                            Projekt_naziv = reader.GetString(5),
                            Datum_obracuna = reader.GetDateTime(6),
                            Ukupni_trosak = reader.GetDouble(7)
                        });
                    }
                }
            }
        }
    }
}
