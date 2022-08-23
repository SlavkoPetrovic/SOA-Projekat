const express = require("express");
const mongoose = require("mongoose");
require("dotenv/config");
const bodyParser = require("body-parser");

const app = express();

//Import Routes
const movieRoute = require("./routes/movies");
const { options } = require("./routes/movies");
app.use(bodyParser.json());
app.use("/movies", movieRoute);

mongoose
  .connect(process.env.DB_CONNECTION)
  .then(() => console.log("Connected to the DB"))
  .catch((e) => console.log(e));

//how do we start listening to the server

app.listen(3000);
