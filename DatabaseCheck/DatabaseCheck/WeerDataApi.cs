using System;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Npgsql;
using Serilog;

public class WeerDataApi
{
    private readonly AppDbContext _context;
    private string _weerApiUrl = "https://weerlive.nl/api/weerlive_api_v2.php?key=655523df03&locatie=Akkrum";
    private string _timeApiUrl = "https://worldtimeapi.org/api/timezone/Europe/Amsterdam";

    public WeerDataApi(AppDbContext context)
    {
        _context = context;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await HaalEnSlaDataOp();  
            Log.Information("Wachten voor de volgende API-oproep...");
            await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);  
        }
    }

    public async Task HaalEnSlaDataOp()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                var weerApiAntwoord = await client.GetAsync(_weerApiUrl);
                weerApiAntwoord.EnsureSuccessStatusCode();
                var weerData = await weerApiAntwoord.Content.ReadAsStringAsync();

                var tijdApiAntwoord = await client.GetAsync(_timeApiUrl);
                tijdApiAntwoord.EnsureSuccessStatusCode();
                var tijdData = await tijdApiAntwoord.Content.ReadAsStringAsync();

                await ZetDataInDatabase(weerData, tijdData);
            }
        }
        catch (Exception ex)
        {
            Log.Information($"Er is een fout opgetreden bij het ophalen van de API-gegevens: {ex.Message}");
        }
    }

    private async Task ZetDataInDatabase(string weerData, string tijdData)
    {
        try
        {
            JObject jsonWeerData = JObject.Parse(weerData);
            var weerInfo = jsonWeerData["liveweer"][0];

            string plaats = (string)weerInfo["plaats"];
            string temperatuurString = (string)weerInfo["temp"];

            if (decimal.TryParse(temperatuurString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal temperatuur))
            {
                JObject jsonTijdData = JObject.Parse(tijdData);
                string apiTijd = (string)jsonTijdData["datetime"];
                Log.Information($"Data opgeslagen: temperatuur={temperatuur}, tijd={apiTijd}, locatie={plaats}");

                var buitentemperatuur = new BuitenTemperatuur
                {
                    Temperatuur = temperatuur,
                    Tijd = apiTijd,
                    Locatie = plaats
                };

                _context.BuitenTemperatuur.Add(buitentemperatuur);
                await _context.SaveChangesAsync();
                Log.Information("Data succesvol opgeslagen in de database.");
            }
            else
            {
                Log.Information("De temperatuurwaarde kon niet worden geconverteerd naar een numeriek type.");
            }
        }
        catch (Exception ex)
        {
            Log.Information($"Er is een fout opgetreden bij het opslaan van gegevens in de database: {ex.Message}");
        }
    }
}


