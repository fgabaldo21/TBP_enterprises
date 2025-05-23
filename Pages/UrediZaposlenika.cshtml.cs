using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Globalization;
using TBP_enterprises.Models;


namespace TBP_enterprises.Pages
{
    public class UrediZaposlenikaModel : PageModel
    {
        [BindProperty]
        public Zaposlenik Zaposlenik { get; set; }
        public List<Uloga> Uloge { get; set; } = new List<Uloga>();

        public void OnGet(int id)
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand(
                    @"SELECT z.id_zaposlenik, z.ime, z.prezime, z.pocetak_zaposlenja, z.zavrsetak_zaposlenja,
                             z.satnica, z.uloga, u.naziv AS naziv_uloge
                      FROM Zaposlenici z
                      LEFT JOIN Uloge u ON z.uloga = u.id_uloga
                      WHERE z.id_zaposlenik = @Id",
                    connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Zaposlenik = new Zaposlenik
                        {
                            Id = reader.GetInt32(0),
                            Ime = reader.GetString(1),
                            Prezime = reader.GetString(2),
                            PocetakZaposlenja = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                            ZavrsetakZaposlenja = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                            Satnica = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
                            Id_uloga = reader.GetInt32(6),
                            NazivUloge = reader.GetString(7)
                        };
                    }
                }

                var ulogeCommand = new Npgsql.NpgsqlCommand("SELECT id_uloga, naziv FROM Uloge", connection);
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
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand(
                    @"UPDATE Zaposlenici 
                      SET ime = @Ime, prezime = @Prezime, pocetak_zaposlenja = @PocetakZaposlenja, 
                          zavrsetak_zaposlenja = @ZavrsetakZaposlenja, satnica = @Satnica, id_uloga = @IdUloga
                      WHERE id_zaposlenik = @Id",
                    connection);

                command.Parameters.AddWithValue("@Ime", Zaposlenik.Ime);
                command.Parameters.AddWithValue("@Prezime", Zaposlenik.Prezime);
                command.Parameters.AddWithValue("@PocetakZaposlenja", Zaposlenik.PocetakZaposlenja);
                command.Parameters.AddWithValue("@ZavrsetakZaposlenja", Zaposlenik.ZavrsetakZaposlenja ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(
                    "@Satnica",
                    string.IsNullOrWhiteSpace(Request.Form["Satnica"])
                        ? (object)DBNull.Value
                        : decimal.Parse(Request.Form["Satnica"].ToString().Replace(',', '.'), CultureInfo.InvariantCulture)
                );
                command.Parameters.AddWithValue("@IdUloga", Zaposlenik.Id_uloga);
                command.Parameters.AddWithValue("@Id", Zaposlenik.Id);

                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Zaposlenici");
        }

        public IActionResult OnPostObrisi()
        {
            string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new Npgsql.NpgsqlCommand("DELETE FROM Zaposlenici WHERE id_zaposlenik = @Id", connection);
                command.Parameters.AddWithValue("@Id", Zaposlenik.Id);
                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Zaposlenici");
        }
    }
}