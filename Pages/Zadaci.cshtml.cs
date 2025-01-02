using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;
using Npgsql;

namespace TBP_enterprises.Pages
{
    public class ZadaciModel : PageModel
    {
        public List<Zadatak> Zadaci { get; set; } = new List<Zadatak>();
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
                        z.id_zadatak, 
                        z.projekt, 
                        p.naziv_projekta, 
                        z.naziv_zadatka, 
                        z.predvidjeni_sati, 
                        z.status, 
                        s.tekst AS naziv_statusa
                      FROM Zadaci z
                      JOIN Projekti p ON z.projekt = p.id_projekt
                      JOIN Status s ON z.status = s.id_status
                      ORDER BY z.id_zadatak
                      LIMIT @VelicinaStranice OFFSET @Offset",
                    connection);

                command.Parameters.AddWithValue("@VelicinaStranice", VelicinaStranice);
                command.Parameters.AddWithValue("@Offset", (stranica - 1) * VelicinaStranice);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Zadaci.Add(new Zadatak
                        {
                            Id = reader.GetInt32(0),
                            Id_projekt = reader.GetInt32(1),
                            Naziv_projekta = reader.GetString(2),
                            Naziv_zadatka = reader.GetString(3),
                            Predvidjeni_sati = reader.GetDecimal(4),
                            Id_status = reader.GetInt32(5),
                            Naziv_statusa = reader.GetString(6)
                        });
                    }
                }
            }
        }
    }
}
