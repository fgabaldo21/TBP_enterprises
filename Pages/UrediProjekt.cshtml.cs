using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Globalization;
using TBP_enterprises.Models;

namespace TBP_enterprises.Pages
{
    public class UrediProjektModel : PageModel
    {
        [BindProperty]
        public Projekt Projekt { get; set; }

        public void OnGet(int id)
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand("SELECT * FROM Projekti WHERE id_projekt = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Projekt = new Projekt
                        {
                            Id = reader.GetInt32(0),
                            Naziv = reader.GetString(1),
                            Datum_pocetka = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                            Datum_zavrsetka = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
                        };
                    }
                }
            }
        }
        
        public IActionResult OnPost()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand(
                    "UPDATE Projekti SET naziv_projekta = @Naziv, datum_pocetka = @Datum_pocetka, datum_zavrsetka = @Datum_zavrsetka WHERE id_projekt = @Id",
                    connection);

                command.Parameters.AddWithValue("@Naziv", Projekt.Naziv);
                command.Parameters.AddWithValue("@Datum_pocetka", Projekt.Datum_pocetka);
                command.Parameters.AddWithValue("@Datum_zavrsetka", Projekt.Datum_zavrsetka ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Id", Projekt.Id);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Projekti");
        }
        
        public IActionResult OnPostObrisi()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand("DELETE FROM Projekti WHERE id_projekt = @Id", connection);
                command.Parameters.AddWithValue("@Id", Projekt.Id);
                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Projekti");
        }
    }
}
