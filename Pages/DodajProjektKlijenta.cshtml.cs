using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;
using Npgsql;
using System.Collections.Generic;

namespace TBP_enterprises.Pages
{
    public class DodajProjektKlijentaModel : PageModel
    {
        [BindProperty]
        public ProjektKlijenta ProjektKlijenta { get; set; } = new ProjektKlijenta();

        public List<Klijent> Klijenti { get; set; } = new List<Klijent>();
        public List<Projekt> Projekti { get; set; } = new List<Projekt>();

        public void OnGet()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var klijentiCommand = new NpgsqlCommand("SELECT id_klijent, naziv_klijenta FROM Klijenti", connection);
                using (var reader = klijentiCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Klijenti.Add(new Klijent
                        {
                            Id = reader.GetInt32(0),
                            Naziv_klijenta = reader.GetString(1)
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
                    "INSERT INTO Projekti_klijenata (klijent, projekt) VALUES (@IdKlijent, @IdProjekt)",
                    connection);

                command.Parameters.AddWithValue("@IdKlijent", ProjektKlijenta.Id_klijent);
                command.Parameters.AddWithValue("@IdProjekt", ProjektKlijenta.Id_projekt);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/ProjektiKlijenata");
        }
    }
}
