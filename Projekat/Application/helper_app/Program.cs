using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

var currentDir = Environment.CurrentDirectory;
using (var streamReader = new StreamReader(Path.Combine(currentDir, @"marvel_movies.csv")))
{
    using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
    {

        var records = csvReader.GetRecords<MarvelMovies>().ToList();
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        var count = 0;
        while (await timer.WaitForNextTickAsync())
        {

            var serializedObject = JsonConvert.SerializeObject(records[count]);
            var movie = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                await httpClient.PostAsync($"http://host.docker.internal:5001/Gateway/postMovie", movie);
            }
            Console.WriteLine(count + " / 63");
            count++;

            if (count > records.Count - 1)
            {
                timer.Dispose();
            }
        }
    }
}


public class MarvelMovies
{
    [Name("Title")]
    public String? Title { get; set; }
    [Name("Distributor")]
    public String? Distributor { get; set; }
    [Name("ReleaseDateUS")]
    public DateTime ReleaseDateUS { get; set; }
    [Name("Budget")]
    public long Budget { get; set; }
    [Name("OpeningWeekendNorthAmerica")]
    public long OpeningWeekendNorthAmerica { get; set; }
    [Name("NorthAmerica")]
    public long NorthAmerica { get; set; }
    [Name("OtherTerritories")]
    public long OtherTerritories { get; set; }
    [Name("Worldwide")]
    public long Worldwide { get; set; }

}


