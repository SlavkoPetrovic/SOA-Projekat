const express = require("express");
const router = express.Router();
const Movie = require("../model/Movie");

//getMovieByTitle
router.get("/:title", async (req, res) => {
  try {
    const movie = await Movie.findOne({ Title: req.params.title });
    if (movie) {
      res.status(200).json(movie);
    } else {
      res.status(404).json("Movie not found!");
    }
  } catch (err) {
    res.json({ message: err });
  }
});
//getRandomMovie
router.get("/", async (req, res) => {
  try {
    const movie = await Movie.aggregate([{ $sample: { size: 1 } }]);
    if (movie) {
      res.status(200).json(movie[0]);
    } else {
      res.status(404).json("Movie not found");
    }
  } catch (error) {
    res.json({ message: error });
  }
});
//deleteAllMovies
router.delete("/", async (req, res) => {
  try {
    await Movie.deleteMany({});
    res.status(200).json("All movies have been successfully deleted!");
  } catch (err) {
    res.json({ message: err });
  }
});
//deleteMovieByTitle
router.delete("/:title", async (req, res) => {
  try {
    var deletedMovie = await Movie.findOneAndRemove({
      Title: req.params.title,
    });
    if (deletedMovie) {
      res
        .status(200)
        .json("Movie " + req.params.title + " has been successfully deleted!");
    } else {
      res.status(404).json("Movie not found!");
    }
  } catch (err) {
    res.json({ message: err });
  }
});
//updateMovieByTitle
router.put("/:title", async (req, res) => {
  try {
    const updatedMovie = await Movie.findOneAndUpdate(
      { Title: req.params.title },
      { $set: { Title: req.body.Title, Distributor: req.body.Distributor } }
    );
    if (updatedMovie) {
      res
        .status(200)
        .json("Movie " + req.body.Title + " has been successfully updated!");
    } else {
      res.status(400).json("Movie not found!");
    }
  } catch (error) {
    res.json({ message: error });
  }
});

/**
 * {
    "Title": "Morbius",
    "Distributor": "Sony Pictures Releasing",
    "ReleaseDateUS": "2022-03-10T22:00:00Z",
    "Budget": 75000000,
    "OpeningWeekendNorthAmerica": 40000000,
    "NorthAmerica": 63000000,
    "OtherTerritories": 100000000,
    "Worldwide": 163000000
}
 */
//postMovie
router.post("/", async (req, res) => {
  const movie = new Movie({
    Title: req.body.Title,
    Distributor: req.body.Distributor,
    ReleaseDateUS: req.body.ReleaseDateUS,
    Budget: req.body.Budget,
    OpeningWeekendNorthAmerica: req.body.OpeningWeekendNorthAmerica,
    NorthAmerica: req.body.NorthAmerica,
    OtherTerritories: req.body.OtherTerritories,
    Worldwide: req.body.Worldwide,
  });
  try {
    const savedMovie = await movie.save();
    res.status(200).json(`Movie ${savedMovie.Title} successfully added!`);
  } catch (err) {
    res.json({ message: err });
  }
});

module.exports = router;
