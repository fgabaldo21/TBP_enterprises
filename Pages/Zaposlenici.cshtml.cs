using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using TBP_enterprises.Models;

public class ZaposleniciModel : PageModel
{
    public List<Zaposlenik> Zaposlenici { get; set; } = new List<Zaposlenik>();
    public int TrenutnaStranica { get; set; }
    public int UkupnoStranica { get; set; }

    public void OnGet(int stranica = 1)
    {
        const int VelicinaStranice = 10;
        TrenutnaStranica = stranica;

        Zaposlenici = new List<Zaposlenik>();

        string connectionString = "Host=localhost;Database=tbp_enterprises;Username=postgres;Password=1234;";
        using (var connection = new Npgsql.NpgsqlConnection(connectionString))
        {
            connection.Open();

            var countCommand = new Npgsql.NpgsqlCommand("SELECT COUNT(*) FROM Zaposlenici", connection);
            int ukupno = (int)(long)countCommand.ExecuteScalar();
            UkupnoStranica = (int)Math.Ceiling((double)ukupno / VelicinaStranice);

            var command = new Npgsql.NpgsqlCommand(
                @"SELECT z.id_zaposlenik, z.ime, z.prezime, z.pocetak_zaposlenja, z.zavrsetak_zaposlenja,
                         z.satnica, z.uloga, u.naziv AS naziv_uloge
                  FROM Zaposlenici z
                  LEFT JOIN Uloge u ON z.uloga = u.id_uloga
                  ORDER BY z.id_zaposlenik
                  LIMIT @VelicinaStranice OFFSET @Offset",
                connection);
            command.Parameters.AddWithValue("@VelicinaStranice", VelicinaStranice);
            command.Parameters.AddWithValue("@Offset", (stranica - 1) * VelicinaStranice);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Zaposlenici.Add(new Zaposlenik
                    {
                        Id = reader.GetInt32(0),
                        Ime = reader.GetString(1),
                        Prezime = reader.GetString(2),
                        PocetakZaposlenja = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                        ZavrsetakZaposlenja = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                        Satnica = reader.GetDecimal(5),
                        Id_uloga = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                        NazivUloge = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                    });
                }
            }
        }
    }
}