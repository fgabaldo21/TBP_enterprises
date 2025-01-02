using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;

namespace TBP_enterprises.Pages
{
    public class UrediKlijentaModel : PageModel
    {
        [BindProperty]
        public Klijent Klijent { get; set; }

        public void OnGet(int id)
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand("SELECT * FROM Klijenti WHERE id_klijent = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Klijent = new Klijent
                        {
                            Id = reader.GetInt32(0),
                            Naziv_klijenta = reader.GetString(1),
                            Kontakt_email = reader.GetString(2)
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
                    "UPDATE Klijenti SET naziv_klijenta= @Naziv_klijenta, kontakt_email = @Kontakt_email WHERE id_klijent = @Id",
                    connection);

                command.Parameters.AddWithValue("@Naziv_klijenta", Klijent.Naziv_klijenta);
                command.Parameters.AddWithValue("@Kontakt_email", Klijent.Kontakt_email);
                command.Parameters.AddWithValue("@Id", Klijent.Id);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Klijenti");
        }

        public IActionResult OnPostObrisi()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand("DELETE FROM Klijenti WHERE id_klijent = @Id", connection);
                command.Parameters.AddWithValue("@Id", Klijent.Id);
                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Klijenti");
        }
    }
}
