using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;
using System.Collections.Generic;
using Npgsql;

namespace TBP_enterprises.Pages
{
    public class DodajObracunModel : PageModel
    {
        [BindProperty]
        public Obracun NoviObracun { get; set; } = new Obracun();

        public List<Zaposlenik> Zaposlenici { get; set; } = new List<Zaposlenik>();
        public List<Projekt> Projekti { get; set; } = new List<Projekt>();

        public void OnGet()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(
                    @"INSERT INTO Obracuni (zaposlenik, projekt, datum_obracuna, ukupni_trosak) 
                      VALUES (@Id_zaposlenik, @Id_projekt, @Datum_obracuna, @Ukupni_trosak)",
                    connection);

                command.Parameters.AddWithValue("@Id_zaposlenik", NoviObracun.Id_zaposlenik);
                command.Parameters.AddWithValue("@Id_projekt", NoviObracun.Id_projekt);
                command.Parameters.AddWithValue("@Datum_obracuna", NoviObracun.Datum_obracuna);
                command.Parameters.AddWithValue("@Ukupni_trosak", NoviObracun.Ukupni_trosak);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Obracuni");
        }
    }
}
