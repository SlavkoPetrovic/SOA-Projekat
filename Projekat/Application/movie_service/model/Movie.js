const mongoose = require("mongoose");

const MovieSchema = mongoose.Schema({
  Title: {
    type: String,
    required: true,
  },
  Distributor: String,
  ReleaseDateUS: Date,
  Budget: Number,
  OpeningWeekendNorthAmerica: Number,
  NorthAmerica: Number,
  OtherTerritories: Number,
  Worldwide: Number,
});

module.exports = mongoose.model("marvel_movies", MovieSchema);
