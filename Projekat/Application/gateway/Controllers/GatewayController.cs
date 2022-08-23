using Microsoft.AspNetCore.Mvc;
using System.Net;
using gateway.Models;
using Newtonsoft.Json;
using System.Text;
using gateway.DTO;
using IMDbApiLib;
using MQTTnet;
using MQTTnet.Client;

namespace gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class GatewayController : ControllerBase
{
    private readonly IConfiguration Configuration;

    public GatewayController(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    [HttpDelete]
    [Route("deleteMovie/{title}")]
    public async Task<IActionResult> deleteMovie(string title)
    {
        using (var httpClient = new HttpClient())
        {
            using (var response = await httpClient.DeleteAsync("http://host.docker.internal:3000/movies/" + title))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                return new JsonResult(
                    new
                    {
                        response = apiResponse
                    }
                );

            }
        }
    }
    [HttpDelete]
    [Route("deleteMovies")]
    public async Task<IActionResult> deleteMovies()
    {
        using (var httpClient = new HttpClient())
        {
            using (var response = await httpClient.DeleteAsync("http://host.docker.internal:3000/movies"))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                return new JsonResult(
                    new
                    {
                        response = apiResponse
                    }
                );

            }
        }
    }
    /**
 * {
    "Title": "Morbius",
    "Distributor": "Sony Pictures Releasing",
    "ReleaseDateUS": "2022-03-10T22:00:00Z",
    "Budget": 75000000,
    "OpeningWeekendNorthAmerica": 40000000,
    "NorthAmerica": 63000000,
    "OtherTerritories": 100000000,
    "Worldwide": 629054379
}
 */
    [HttpPost]
    [Route("postMovie")]
    public async Task<IActionResult> postMovie([FromBody] MarvelMovies newMovie)
    {
        using (var httpClient = new HttpClient())
        {
            var c = JsonConvert.SerializeObject(newMovie);
            StringContent content = new StringContent(c, Encoding.UTF8, "application/json");
            using (var response = await httpClient.PostAsync("http://host.docker.internal:3000/movies", content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var mqtt = new
                {
                    Title = newMovie.Title,
                    Distributor = newMovie.Distributor,
                    Budget = newMovie.Budget,
                    OpeningWeekendNorthAmerica = newMovie.OpeningWeekendNorthAmerica,
                    NorthAmerica = newMovie.NorthAmerica,
                    OtherTerritories = newMovie.OtherTerritories,
                    Worldwide = newMovie.Worldwide
                };
                await publishMqtt(JsonConvert.SerializeObject(mqtt));
                return new JsonResult(
                    new
                    {
                        response = apiResponse
                    }
                );
            }
        }
    }
    [HttpPut]
    [Route("updateMovie/{title}")]
    public async Task<IActionResult> updateMovie(string title, UpdateMovies updatedMovie)
    {
        using (var httpClient = new HttpClient())
        {
            var c = JsonConvert.SerializeObject(updatedMovie);
            StringContent content = new StringContent(c, Encoding.UTF8, "application/json");
            using (var response = await httpClient.PutAsync("http://host.docker.internal:3000/movies/" + title, content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return new JsonResult(
                    new
                    {
                        response = apiResponse
                    }

                );
            }
        }
    }
    [HttpGet]
    [Route("getMovie/{title}")]
    public async Task<IActionResult> getMovie(string title)
    {
        var returnValue = new Response();
        using (var httpClient = new HttpClient())
        {
            using (var response = await httpClient.GetAsync("http://host.docker.internal:3000/movies/" + title))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    returnValue.Movie = await response.Content.ReadFromJsonAsync<MarvelMovies>();
                }
                else
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return new JsonResult(
                        new
                        {
                            response = apiResponse
                        }
                    );
                }
            }
        }
        var apiLib = new ApiLib(Configuration["IMDBapiKey"]);
        var data = await apiLib.SearchMovieAsync(title);
        var ratings = await apiLib.RatingsAsync(data.Results[0].Id);
        returnValue.Reviews = new
        {
            imdb = ratings.IMDb == "" ? "?" + "/10" : ratings.IMDb + "/10",
            fimAffinity = ratings.FilmAffinity == "" ? "?" + "/10" : ratings.FilmAffinity + "/10",
            theMovieDb = ratings.TheMovieDb == "" ? "?" + "/10" : ratings.TheMovieDb + "/10",
            metacritic = ratings.Metacritic == "" ? "?" + "/100" : ratings.Metacritic + "/100",
            rottenTomatoes = ratings.RottenTomatoes == "" ? "?" + "/100" : ratings.RottenTomatoes + "/100"
        };
        return new JsonResult(returnValue);
    }

    [HttpGet]
    [Route("getRandomMovie")]
    public async Task<IActionResult> getRandomMovie()
    {
        var returnValue = new Response();
        using (var httpClient = new HttpClient())
        {
            //host.docker.internal
            using (var response = await httpClient.GetAsync("http://host.docker.internal:3000/movies"))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    returnValue.Movie = await response.Content.ReadFromJsonAsync<MarvelMovies>();
                }
                else
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return new JsonResult(
                        new
                        {
                            response = apiResponse
                        }
                    );
                }
            }
        }
        var apiLib = new ApiLib(Configuration["IMDBapiKey"]);
        var data = await apiLib.SearchMovieAsync(returnValue?.Movie?.Title);
        var ratings = await apiLib.RatingsAsync(data.Results[0].Id);
        returnValue!.Reviews = new
        {
            imdb = ratings.IMDb == "" ? "?" + "/10" : ratings.IMDb + "/10",
            fimAffinity = ratings.FilmAffinity == "" ? "?" + "/10" : ratings.FilmAffinity + "/10",
            theMovieDb = ratings.TheMovieDb == "" ? "?" + "/10" : ratings.TheMovieDb + "/10",
            metacritic = ratings.Metacritic == "" ? "?" + "/100" : ratings.Metacritic + "/100",
            rottenTomatoes = ratings.RottenTomatoes == "" ? "?" + "/100" : ratings.RottenTomatoes + "/100"
        };


        return new JsonResult(returnValue);
    }
    static async Task publishMqtt(string payload)
    {
        var mqttFactory = new MqttFactory();
        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer("broker.emqx.io", 1883)
            .Build();

            await mqttClient.ConnectAsync(options, CancellationToken.None);

            var message = new MqttApplicationMessageBuilder()
            .WithTopic("inputMQTT")
            .WithPayload(payload)
            .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);
            // await mqttClient.DisconnectAsync();
        }

    }
}