using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using TBP_enterprises.Models;
using System.Collections.Generic;

namespace TBP_enterprises.Pages
{
    public class UrediProjektKlijentaModel : PageModel
    {
        [BindProperty]
        public ProjektKlijenta ProjektKlijenta { get; set; }

        public List<Projekt> Projekti { get; set; } = new List<Projekt>();

        [BindProperty]
        public int OriginalIdProjekt { get; set; }

        public void OnGet(int idKlijent, int idProjekt)
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(
                    @"SELECT pk.klijent, k.naziv_klijenta AS naziv_klijenta, pk.projekt, p.naziv_projekta AS naziv_projekta
                      FROM Projekti_klijenata pk
                      JOIN Klijenti k ON pk.klijent = k.id_klijent
                      JOIN Projekti p ON pk.projekt = p.id_projekt
                      WHERE pk.klijent = @IdKlijent AND pk.projekt = @IdProjekt",
                    connection);
                command.Parameters.AddWithValue("@IdKlijent", idKlijent);
                command.Parameters.AddWithValue("@IdProjekt", idProjekt);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ProjektKlijenta = new ProjektKlijenta
                        {
                            Id_klijent = reader.GetInt32(0),
                            Naziv_klijenta = reader.GetString(1),
                            Id_projekt = reader.GetInt32(2),
                            Naziv_projekta = reader.GetString(3)
                        };
                        OriginalIdProjekt = reader.GetInt32(2);
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
                    @"UPDATE Projekti_klijenata 
                      SET projekt = @IdProjekt 
                      WHERE klijent = @IdKlijent AND projekt = @OriginalIdProjekt",
                    connection);

                command.Parameters.AddWithValue("@IdKlijent", ProjektKlijenta.Id_klijent);
                command.Parameters.AddWithValue("@OriginalIdProjekt", OriginalIdProjekt);
                command.Parameters.AddWithValue("@IdProjekt", ProjektKlijenta.Id_projekt);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/ProjektiKlijenata");
        }
    }
}
