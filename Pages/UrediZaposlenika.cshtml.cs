using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Globalization;
using TBP_enterprises.Models;

public class UrediZaposlenikaModel : PageModel
{
    [BindProperty]
    public Zaposlenik Zaposlenik { get; set; }

    public void OnGet(int id)
    {
        string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
        using (var connection = new Npgsql.NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new Npgsql.NpgsqlCommand("SELECT * FROM Zaposlenici WHERE id_zaposlenik = @Id", connection);
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
                        Satnica = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5)
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
                "UPDATE Zaposlenici SET ime = @Ime, prezime = @Prezime, pocetak_zaposlenja = @PocetakZaposlenja, zavrsetak_zaposlenja = @ZavrsetakZaposlenja, satnica = @Satnica WHERE id_zaposlenik = @Id",
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
            command.Parameters.AddWithValue("@Id", Zaposlenik.Id);

            command.ExecuteNonQuery();
        }

        return RedirectToPage("/Zaposlenici");
    }
}
