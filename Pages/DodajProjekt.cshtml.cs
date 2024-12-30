using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;

namespace TBP_enterprises.Pages
{
    public class DodajProjektModel : PageModel
    {
        [BindProperty]
        public Projekt NoviProjekt { get; set; } = new Projekt();

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
                    "INSERT INTO Projekti (naziv_projekta, datum_pocetka, datum_zavrsetka) VALUES (@Naziv, @Datum_pocetka, @Datum_zavrsetka)",
                    connection);
                command.Parameters.AddWithValue("@Naziv", NoviProjekt.Naziv);
                command.Parameters.AddWithValue("@Datum_pocetka", NoviProjekt.Datum_pocetka);
                command.Parameters.AddWithValue("@Datum_zavrsetka", NoviProjekt.Datum_zavrsetka ?? (object)DBNull.Value); 

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Projekti");
        }
    }
}
