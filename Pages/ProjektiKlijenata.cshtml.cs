using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using TBP_enterprises.Models;
using System.Collections.Generic;

namespace TBP_enterprises.Pages
{
    public class ProjektiKlijenataModel : PageModel
    {
        public List<ProjektKlijenta> ProjektiKlijenata { get; set; } = new List<ProjektKlijenta>();
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
                var countCommand = new NpgsqlCommand("SELECT COUNT(*) FROM Projekti_klijenata", connection);
                int ukupno = (int)(long)countCommand.ExecuteScalar();
                UkupnoStranica = (int)Math.Ceiling((double)ukupno / VelicinaStranice);
                var command = new NpgsqlCommand(
                    @"SELECT pk.klijent, k.naziv_klijenta AS naziv_klijenta, pk.projekt, p.naziv_projekta AS naziv_projekta
                      FROM Projekti_klijenata pk
                      JOIN Klijenti k ON pk.klijent = k.id_klijent
                      JOIN Projekti p ON pk.projekt = p.id_projekt
                      ORDER BY pk.klijent
                      LIMIT @VelicinaStranice OFFSET @Offset",
                    connection);
                command.Parameters.AddWithValue("@VelicinaStranice", VelicinaStranice);
                command.Parameters.AddWithValue("@Offset", (stranica - 1) * VelicinaStranice);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProjektiKlijenata.Add(new ProjektKlijenta
                        {
                            Id_klijent = reader.GetInt32(0),
                            Naziv_klijenta = reader.GetString(1),
                            Id_projekt = reader.GetInt32(2),
                            Naziv_projekta = reader.GetString(3)
                        });
                    }
                }
            }
        }
    }
}
