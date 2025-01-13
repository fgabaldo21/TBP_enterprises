using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using TBP_enterprises.Models;
using System.Globalization;
using Npgsql;

namespace TBP_enterprises.Pages
{
    public class DodajZaposlenikaModel : PageModel
    {
        [BindProperty]
        public Zaposlenik NoviZaposlenik { get; set; } = new Zaposlenik();
        public List<Uloga> Uloge { get; set; } = new List<Uloga>();

        public void OnGet()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT id_uloga, naziv FROM Uloge", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Uloge.Add(new Uloga
                        {
                            Id = reader.GetInt32(0),
                            Naziv_uloge = reader.GetString(1)
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
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand(
                    "INSERT INTO Zaposlenici (ime, prezime, pocetak_zaposlenja, zavrsetak_zaposlenja, satnica, uloga) VALUES (@Ime, @Prezime, @PocetakZaposlenja, @ZavrsetakZaposlenja, @Satnica, @IdUloga)",
                    connection);
                command.Parameters.AddWithValue("@Ime", NoviZaposlenik.Ime);
                command.Parameters.AddWithValue("@Prezime", NoviZaposlenik.Prezime);
                command.Parameters.AddWithValue("@PocetakZaposlenja", NoviZaposlenik.PocetakZaposlenja);
                command.Parameters.AddWithValue("@ZavrsetakZaposlenja", NoviZaposlenik.ZavrsetakZaposlenja ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(
                    "@Satnica",
                    string.IsNullOrWhiteSpace(Request.Form["Satnica"])
                        ? (object)DBNull.Value
                        : decimal.Parse(Request.Form["Satnica"].ToString().Replace(',', '.'), CultureInfo.InvariantCulture)
                );
                command.Parameters.AddWithValue("@IdUloga", NoviZaposlenik.Id_uloga);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Zaposlenici");
        }
    }
}