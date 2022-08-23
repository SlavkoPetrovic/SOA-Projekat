const mqtt = require("mqtt");
const express = require("express");
const { InfluxDB } = require("@influxdata/influxdb-client");

const app = express();
//INFLUXDB
const token =
  "605RiYlHd8zfPF3lwY91XFK7l0yAj6Kc0kXjkD8UB90buszDGnsb5cWLABgVF4pxHpzp8Hoxh4_e_4txl7ErFg==";
const org = "slavko";
const bucket = "slavko";

const client = new InfluxDB({
  url: "http://host.docker.internal:8086",
  token: token,
});

const { Point } = require("@influxdata/influxdb-client");
const writeApi = client.getWriteApi(org, bucket);
writeApi.useDefaultTags({ host: "EdgeX" });

//MQTT

var mqttClient = mqtt.connect("tcp://broker.emqx.io:1883", { clean: true });

mqttClient.on("connect", () => {
  mqttClient.subscribe("projekatIII", (err) => {
    if (err) {
      console.log("Error while trying to subscribe to the topic", err);
    } else {
      console.log("Mqtt client subscribed to the topic");
    }
  });
});

mqttClient.on("message", function (topic, message) {
  var pom = JSON.parse(message).readings[0];
  var name = pom.name;
  var value = pom.value;
  var vrednost = new Buffer.from(value, "base64").readDoubleBE(0);

  console.log(
    "Pristigao je podatak " + name + " cija je vrednost: " + vrednost
  );

  var point = new Point("edgex").floatField(name, vrednost);
  writeApi.writePoint(point);
});

app.listen(3000);
