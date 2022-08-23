namespace gateway.DTO;
using gateway.Models;

public class Response
{
    public MarvelMovies? Movie { get; set; }
    public dynamic? Reviews { get; set; }
}