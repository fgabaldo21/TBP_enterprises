using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBP_enterprises.Models;
using System.Collections.Generic;
using Npgsql;

namespace TBP_enterprises.Pages
{
    public class DodajZadatakModel : PageModel
    {
        [BindProperty]
        public Zadatak NoviZadatak { get; set; } = new Zadatak();

        public List<Projekt> Projekti { get; set; } = new List<Projekt>();
        public List<Status> Statusi { get; set; } = new List<Status>();

        public void OnGet()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var projektCommand = new NpgsqlCommand("SELECT id_projekt, naziv_projekta FROM Projekti", connection);
                using (var reader = projektCommand.ExecuteReader())
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

                var statusCommand = new NpgsqlCommand("SELECT id_status, tekst FROM Status", connection);
                using (var reader = statusCommand.ExecuteReader())
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
            if (!ModelState.IsValid)
            {
                return Page();
            }


            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;IncludeErrorDetail=true;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(
                    "INSERT INTO Zadaci (projekt, naziv_zadatka, predvidjeni_sati, status) VALUES (@Id_projekt, @Naziv_zadatka, @Predvidjeni_sati, @Status)",
                    connection);
                command.Parameters.AddWithValue("@Id_projekt", NoviZadatak.Id_projekt);
                command.Parameters.AddWithValue("@Naziv_zadatka", NoviZadatak.Naziv_zadatka);
                command.Parameters.AddWithValue("@Predvidjeni_sati", NoviZadatak.Predvidjeni_sati);
                command.Parameters.AddWithValue("@Status", NoviZadatak.Id_status);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Zadaci");
        }
    }
}