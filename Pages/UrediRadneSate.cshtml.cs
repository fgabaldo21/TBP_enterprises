using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using TBP_enterprises.Models;
using System.Collections.Generic;

namespace TBP_enterprises.Pages
{
    public class UrediRadneSateModel : PageModel
    {
        [BindProperty]
        public RadniSat RadniSat { get; set; }

        public List<Zaposlenik> Zaposlenici { get; set; } = new List<Zaposlenik>();
        public List<Zadatak> Zadaci { get; set; } = new List<Zadatak>();

        public void OnGet(int id)
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var radniSatCommand = new NpgsqlCommand(
                    @"SELECT rs.id_log, rs.zaposlenik, z.ime, z.prezime, rs.zadatak, zd.naziv_zadatka, rs.datum, rs.odradjeni_sati
                      FROM Radni_sati rs
                      JOIN Zaposlenici z ON rs.zaposlenik = z.id_zaposlenik
                      JOIN Zadaci zd ON rs.zadatak = zd.id_zadatak
                      WHERE rs.id_log = @Id",
                    connection);
                radniSatCommand.Parameters.AddWithValue("@Id", id);

                using (var reader = radniSatCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        RadniSat = new RadniSat
                        {
                            Id = reader.GetInt32(0),
                            Id_zaposlenik = reader.GetInt32(1),
                            Zaposlenik_ime = reader.GetString(2),
                            Zaposlenik_prezime = reader.GetString(3),
                            Id_zadatak = reader.GetInt32(4),
                            Naziv_zadatka = reader.GetString(5),
                            Datum = reader.GetDateTime(6),
                            Odradjeni_sati = reader.GetInt32(7)
                        };
                    }
                }

                var zaposleniciCommand = new NpgsqlCommand("SELECT id_zaposlenik, ime, prezime FROM Zaposlenici", connection);
                using (var reader = zaposleniciCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Zaposlenici.Add(new Zaposlenik
                        {
                            Id = reader.GetInt32(0),
                            Ime = reader.GetString(1),
                            Prezime = reader.GetString(2)
                        });
                    }
                }

                var zadaciCommand = new NpgsqlCommand("SELECT id_zadatak, naziv_zadatka FROM Zadaci", connection);
                using (var reader = zadaciCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Zadaci.Add(new Zadatak
                        {
                            Id = reader.GetInt32(0),
                            Naziv_zadatka = reader.GetString(1)
                        });
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    @"UPDATE Radni_sati
                      SET zaposlenik = @Id_zaposlenik, zadatak = @Id_zadatak, datum = @Datum, odradjeni_sati = @Odradjeni_sati
                      WHERE id_log = @Id",
                    connection);
                command.Parameters.AddWithValue("@Id", RadniSat.Id);
                command.Parameters.AddWithValue("@Id_zaposlenik", RadniSat.Id_zaposlenik);
                command.Parameters.AddWithValue("@Id_zadatak", RadniSat.Id_zadatak);
                command.Parameters.AddWithValue("@Datum", RadniSat.Datum);
                command.Parameters.AddWithValue("@Odradjeni_sati", RadniSat.Odradjeni_sati);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/RadniSati");
        }

        public IActionResult OnPostObrisi()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand("DELETE FROM Radni_sati WHERE id_log = @Id", connection);
                command.Parameters.AddWithValue("@Id", RadniSat.Id);
                command.ExecuteNonQuery();
            }

            return RedirectToPage("/RadniSati");
        }
    }
}
