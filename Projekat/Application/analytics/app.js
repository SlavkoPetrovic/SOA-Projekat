//ovde treba subscribe da ide
const mqtt = require("mqtt");
const express = require("express");
const grpc = require("@grpc/grpc-js");
const protoLoader = require("@grpc/proto-loader");
const PROTO_PATH = __dirname + "/greet.proto";
const { InfluxDB } = require("@influxdata/influxdb-client");

// You can generate an API token from the "API Tokens Tab" in the UI
const token =
  "r7Otfr-WO7XdNlFxwSUU5Vs5uf8DhIUfmIcJfcvacaXdbQ-LhZK-9jd901btqLgi4hKqCpTv9ZcxOOl3Sc4l8g==";
const org = "slavko";
const bucket = "slavko";

const client = new InfluxDB({
  url: "http://host.docker.internal:8086",
  token: token,
});

const { Point } = require("@influxdata/influxdb-client");
const writeApi = client.getWriteApi(org, bucket);
writeApi.useDefaultTags({ host: "host1" });

const packageDefinition = protoLoader.loadSync(PROTO_PATH, {
  keepCase: true,
  longs: String,
  enums: String,
  defaults: true,
  oneofs: true,
});

let greet_proto = grpc.loadPackageDefinition(packageDefinition).greet;
const grpcClient = new greet_proto.Greeter(
  "host.docker.internal:5011",
  grpc.credentials.createInsecure()
);

const app = express();

var mqttClient = mqtt.connect("tcp://broker.emqx.io:1883");

mqttClient.on("connect", () => {
  mqttClient.subscribe("outputMQTT", (err) => {
    if (err) {
      console.log("Error while trying to subscribe to the topic", err);
    } else {
      console.log("Mqtt client subscribed to the topic");
    }
  });
});
mqttClient.on("message", function (topic, message) {
  console.log(
    "mqttOutput recieved filtered movie from eKuiper : " + message.toString()
  );

  var e = JSON.parse(message.toString());
  var point = new Point("movie")
    .uintField("Worldwide", parseInt(e[0].Worldwide))
    .intField("Budget", parseInt(e[0].Budget))
    .intField(
      "OpeningWeekendNorthAmerica",
      parseInt(e[0].OpeningWeekendNorthAmerica)
    )
    .intField("NorthAmerica", parseInt(e[0].NorthAmerica))
    .intField("OtherTerritories", parseInt(e[0].OtherTerritories))
    .stringField("Title", e[0].Title)
    .stringField("Distributor", e[0].Distributor);
  writeApi.writePoint(point);

  grpcClient.Notify(e[0], (err) => {
    if (err) {
      console.log("Grpc error: ", err);
    }
  });
});

app.listen(5003);
