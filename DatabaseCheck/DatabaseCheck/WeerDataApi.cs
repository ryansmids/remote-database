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
        Log.Information("WeerData ontvangen, verwerken begint.");
        JObject jsonWeerData = JObject.Parse(weerData);
        var weerInfo = jsonWeerData["liveweer"][0];

        string plaats = (string)weerInfo["plaats"];
        string temperatuurString = (string)weerInfo["temp"];

        Log.Information($"Plaats: {plaats}, TemperatuurString: {temperatuurString}");

        if (decimal.TryParse(temperatuurString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal temperatuur))
        {
            JObject jsonTijdData = JObject.Parse(tijdData);
            string apiTijd = (string)jsonTijdData["datetime"];  // Dit is de tijd als string

            // Log de ontvangen tijdstring
            Log.Information($"Ontvangen apiTijd: {apiTijd}");

            // Direct de string omzetten naar een DateTime met specifiek formaat
            DateTime tijd;
            try
            {
                tijd = DateTime.ParseExact(apiTijd, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                // Zet de DateTime expliciet om naar UTC
                tijd = DateTime.SpecifyKind(tijd, DateTimeKind.Utc);
            }
            catch (FormatException)
            {
                Log.Error($"Tijdstring kon niet worden geconverteerd naar DateTime: {apiTijd}");
                throw;
            }

            Log.Information($"Gekonverteerde tijd naar UTC: {tijd}");

            var buitentemperatuur = new BuitenTemperatuur
            {
                Temperatuur = temperatuur,
                Tijd = tijd,  // Direct opslaan als DateTime in UTC
                Locatie = plaats
            };

            _context.BuitenTemperatuur.Add(buitentemperatuur);
            await _context.SaveChangesAsync();
            Log.Information("Data succesvol opgeslagen in de database.");
        }
        else
        {
            Log.Warning("De temperatuurwaarde kon niet worden geconverteerd naar een numeriek type.");
        }
    }
    catch (Exception ex)
    {
        Log.Error($"Er is een fout opgetreden bij het opslaan van gegevens in de database: {ex}");
    }
}




}


