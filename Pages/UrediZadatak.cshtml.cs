using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using TBP_enterprises.Models;

namespace TBP_enterprises.Pages
{
    public class UrediZadatakModel : PageModel
    {
        [BindProperty]
        public Zadatak Zadatak { get; set; }
        public List<Projekt> Projekti { get; set; } = new List<Projekt>();
        public List<Status> Statusi { get; set; } = new List<Status>();

        public void OnGet(int id)
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var zadatakCommand = new Npgsql.NpgsqlCommand("SELECT * FROM Zadaci WHERE id_zadatak = @Id", connection);
                zadatakCommand.Parameters.AddWithValue("@Id", id);

                using (var reader = zadatakCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Zadatak = new Zadatak
                        {
                            Id = reader.GetInt32(0),
                            Id_projekt = reader.GetInt32(1),
                            Naziv_zadatka = reader.GetString(2),
                            Predvidjeni_sati = reader.GetDecimal(3),
                            Id_status = reader.GetInt32(4)
                        };
                    }
                }

                var projektiCommand = new Npgsql.NpgsqlCommand("SELECT id_projekt, naziv_projekta FROM Projekti", connection);
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

                var statusiCommand = new Npgsql.NpgsqlCommand("SELECT id_status, tekst FROM Status", connection);
                using (var reader = statusiCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Statusi.Add(new Status
                        {
                            Id_status = reader.GetInt32(0),
                            Naziv_status = reader.GetString(1)
                        });
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
                    "UPDATE Zadaci SET naziv_zadatka = @Naziv_zadatka, projekt = @Id_projekt, predvidjeni_sati = @Predvidjeni_sati, status = @Id_status WHERE id_zadatak = @Id",
                    connection);

                command.Parameters.AddWithValue("@Naziv_zadatka", Zadatak.Naziv_zadatka);
                command.Parameters.AddWithValue("@Id_projekt", Zadatak.Id_projekt);
                command.Parameters.AddWithValue("@Predvidjeni_sati", Zadatak.Predvidjeni_sati);
                command.Parameters.AddWithValue("@Id_status", Zadatak.Id_status);
                command.Parameters.AddWithValue("@Id", Zadatak.Id);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Zadaci");
        }

        public IActionResult OnPostObrisi()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand("DELETE FROM Zadaci WHERE id_zadatak = @Id", connection);
                command.Parameters.AddWithValue("@Id", Zadatak.Id);
                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Zadaci");
        }
    }
}
