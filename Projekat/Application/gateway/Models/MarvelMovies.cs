namespace gateway.Models;

public class MarvelMovies
{
    public String? Title { get; set; }
    public String? Distributor { get; set; }
    public DateTime ReleaseDateUS { get; set; }
    public long Budget { get; set; }
    public long OpeningWeekendNorthAmerica { get; set; }
    public long NorthAmerica { get; set; }
    public long OtherTerritories { get; set; }
    public long Worldwide { get; set; }
}