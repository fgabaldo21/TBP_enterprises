using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TBP_enterprises.Models;
using Npgsql;

namespace TBP_enterprises.Pages
{
    public class RadniSatiModel : PageModel
    {
        public List<RadniSat> RadniSati { get; set; } = new List<RadniSat>();
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

                var countCommand = new Npgsql.NpgsqlCommand("SELECT COUNT(*) FROM Projekti", connection);
                int ukupno = (int)(long)countCommand.ExecuteScalar();
                UkupnoStranica = (int)Math.Ceiling((double)ukupno / VelicinaStranice);

                var command = new NpgsqlCommand(
                    @"SELECT 
                        rs.id_log, 
                        rs.zaposlenik, 
                        z.ime, 
                        z.prezime, 
                        rs.zadatak, 
                        zd.naziv_zadatka, 
                        rs.datum, 
                        rs.odradjeni_sati
                      FROM Radni_sati rs
                      JOIN Zaposlenici z ON rs.zaposlenik = z.id_zaposlenik
                      JOIN Zadaci zd ON rs.zadatak = zd.id_zadatak
                      ORDER BY rs.id_log
                      LIMIT @VelicinaStranice OFFSET @Offset",
                    connection);
                command.Parameters.AddWithValue("@VelicinaStranice", VelicinaStranice);
                command.Parameters.AddWithValue("@Offset", (stranica - 1) * VelicinaStranice);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RadniSati.Add(new RadniSat
                        {
                            Id = reader.GetInt32(0),
                            Id_zaposlenik = reader.GetInt32(1),
                            Zaposlenik_ime = reader.GetString(2),
                            Zaposlenik_prezime = reader.GetString(3),
                            Id_zadatak = reader.GetInt32(4),
                            Naziv_zadatka = reader.GetString(5),
                            Datum = reader.GetDateTime(6),
                            Odradjeni_sati = reader.GetInt32(7)
                        });
                    }
                }
            }
        }
    }
}
