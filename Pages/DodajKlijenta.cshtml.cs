using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;

namespace TBP_enterprises.Pages
{
    public class DodajKlijentaModel : PageModel
    {
        [BindProperty]
        public Klijent NoviKlijent { get; set; } = new Klijent();

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
                    "INSERT INTO Klijenti (naziv_klijenta, kontakt_email) VALUES (@Naziv_klijenta, @Kontakt_email)",
                    connection);
                command.Parameters.AddWithValue("@Naziv_klijenta", NoviKlijent.Naziv_klijenta);
                command.Parameters.AddWithValue("@Kontakt_email", NoviKlijent.Kontakt_email);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Klijenti");
        }
    }
}
