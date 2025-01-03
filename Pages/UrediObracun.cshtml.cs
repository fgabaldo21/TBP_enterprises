using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;
using System.Collections.Generic;
using Npgsql;

namespace TBP_enterprises.Pages
{
    public class UrediObracunModel : PageModel
    {
        [BindProperty]
        public Obracun Obracun { get; set; }

        public List<Zaposlenik> Zaposlenici { get; set; } = new List<Zaposlenik>();
        public List<Projekt> Projekti { get; set; } = new List<Projekt>();

        public void OnGet(int id)
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var obracunCommand = new NpgsqlCommand(
                    @"SELECT o.id_obracun, o.zaposlenik, z.ime, z.prezime, o.projekt, p.naziv_projekta, 
                      o.datum_obracuna, o.ukupni_trosak 
                      FROM Obracuni o
                      JOIN Zaposlenici z ON o.zaposlenik = z.id_zaposlenik
                      JOIN Projekti p ON o.projekt = p.id_projekt
                      WHERE o.id_obracun = @Id",
                    connection);
                obracunCommand.Parameters.AddWithValue("@Id", id);

                using (var reader = obracunCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Obracun = new Obracun
                        {
                            Id = reader.GetInt32(0),
                            Id_zaposlenik = reader.GetInt32(1),
                            Zaposlenik_ime = reader.GetString(2),
                            Zaposlenik_prezime = reader.GetString(3),
                            Id_projekt = reader.GetInt32(4),
                            Projekt_naziv = reader.GetString(5),
                            Datum_obracuna = reader.GetDateTime(6),
                            Ukupni_trosak = reader.GetDouble(7)
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

                var projektiCommand = new NpgsqlCommand("SELECT id_projekt, naziv_projekta FROM Projekti", connection);
                using (var reader = projektiCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Projekti.Add(new Projekt
                        {
                            Id = reader.GetInt32(0),
                            Naziv = reader.GetString(1)
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
                    @"UPDATE Obracuni 
                      SET zaposlenik = @Id_zaposlenik, projekt = @Id_projekt, datum_obracuna = @Datum_obracuna, 
                          ukupni_trosak = @Ukupni_trosak 
                      WHERE id_obracun = @Id",
                    connection);
                command.Parameters.AddWithValue("@Id", Obracun.Id);
                command.Parameters.AddWithValue("@Id_zaposlenik", Obracun.Id_zaposlenik);
                command.Parameters.AddWithValue("@Id_projekt", Obracun.Id_projekt);
                command.Parameters.AddWithValue("@Datum_obracuna", Obracun.Datum_obracuna);
                command.Parameters.AddWithValue("@Ukupni_trosak", Obracun.Ukupni_trosak);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Obracuni");
        }
        public IActionResult OnPostObrisi()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand("DELETE FROM Obracuni WHERE id_obracun = @Id", connection);
                command.Parameters.AddWithValue("@Id", Obracun.Id);
                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Obracuni");
        }
    }
}
