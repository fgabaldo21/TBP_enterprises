using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using TBP_enterprises.Models;

namespace TBP_enterprises.Pages
{
    public class UrediUloguModel : PageModel
    {
        [BindProperty]
        public string ZaposlenikImePrezime { get; set; }
        [BindProperty]
        public int TrenutnaUlogaId { get; set; }
        [BindProperty]
        public List<Uloga> Uloge { get; set; } = new List<Uloga>();
        [BindProperty]
        public int NovaUloga { get; set; }
        public int ZaposlenikId { get; set; }
        public void OnGet(int zaposlenikId, int trenutnaUlogaId)
        {
            ZaposlenikId = zaposlenikId;
            TrenutnaUlogaId = trenutnaUlogaId;

            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var zaposlenikCommand = new NpgsqlCommand(
                    "SELECT ime, prezime FROM Zaposlenici WHERE id_zaposlenik = @Id", connection);
                zaposlenikCommand.Parameters.AddWithValue("@Id", ZaposlenikId);
                using (var reader = zaposlenikCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ZaposlenikImePrezime = $"{reader.GetString(0)} {reader.GetString(1)}";
                    }
                    else
                    {
                        ZaposlenikImePrezime = "Nepoznat zaposlenik";
                    }
                }

                var ulogeCommand = new NpgsqlCommand("SELECT id_uloga, naziv FROM Uloge", connection);
                using (var reader = ulogeCommand.ExecuteReader())
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
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "UPDATE Uloge_zaposlenika SET uloga = @NovaUloga WHERE zaposlenik = @ZaposlenikId AND uloga = @TrenutnaUlogaId",
                    connection);
                command.Parameters.AddWithValue("@NovaUloga", NovaUloga);
                command.Parameters.AddWithValue("@ZaposlenikId", ZaposlenikId);
                command.Parameters.AddWithValue("@TrenutnaUlogaId", TrenutnaUlogaId);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/UlogeZaposlenika");
        }
    }
}
