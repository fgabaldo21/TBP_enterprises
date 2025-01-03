using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;
using Npgsql;
using System.Collections.Generic;

namespace TBP_enterprises.Pages
{
    public class UnosRadnihSatiModel : PageModel
    {
        [BindProperty]
        public RadniSat NoviRadniSat { get; set; } = new RadniSat();

        public List<Zaposlenik> Zaposlenici { get; set; } = new List<Zaposlenik>();
        public List<Zadatak> Zadaci { get; set; } = new List<Zadatak>();

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "INSERT INTO Radni_sati (zaposlenik, zadatak, datum, odradjeni_sati) VALUES (@Id_zaposlenik, @Id_zadatak, @Datum, @Odradjeni_sati)",
                    connection);

                command.Parameters.AddWithValue("@Id_zaposlenik", NoviRadniSat.Id_zaposlenik);
                command.Parameters.AddWithValue("@Id_zadatak", NoviRadniSat.Id_zadatak);
                command.Parameters.AddWithValue("@Datum", NoviRadniSat.Datum);
                command.Parameters.AddWithValue("@Odradjeni_sati", NoviRadniSat.Odradjeni_sati);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/RadniSati");
        }
    }
}
