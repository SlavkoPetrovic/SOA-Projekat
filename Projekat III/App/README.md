
# [SOA - Projekat 3](https://cs.elfak.ni.ac.rs/nastava/pluginfile.php/43854/mod_resource/content/2/SOA%20-%20Projekat%203%20-%202022.pdf)

Simulacija sensor podataka, mera kvaliteta vode

[Dataset ](https://www.kaggle.com/datasets/adityakadiwal/water-potability) koji je koriscen


## Pokretanje

Pozicionirati se u folderu Projekat III/App

```bash
 cd "Projekat III"/App
```
Startovati aplikaciju komandom
```bash
 docker compose up --build
```
Ukoliko se prvi put pokrece aplikacija pozicionirati se deviceCreation
```bash
 cd deviceCreation
```
I startovati skripte
```bash
py createSensorCluster.py
py createRESTDevice.py
```
U browseru otvoriti 
http://localhost:48082/api/v1/device
i kopirati command url u ```monitoring/Program.cs``` na odgovarajuce mestom izmeniti ```edgex-core-command``` sa ```host.docker.internal``` 

Startovati aplikaciju komandom
```bash
 docker compose up --build
```

Za popunjavanje baze podataka pozicionirati se u folderu sensorDataGeneration
```bash
 cd sensorDataGeneration
```
I pokrenuti aplikaciju komandom
```bash
py genSensorData.py
```

   
## API Reference

[SwaggerUI ](http://localhost:7049/swagger/index.html)

#### Update ph limits

```http
  POST /Limits/ph/{min}/{max}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `min` | `float` | **Required**. New minimum limit of ph |
| `max` | `float` | **Required**. New maximum limit of ph |

#### Update Organic_carbon​ limits

```http
  POST ​/Limits​/Organic_carbon​/{min}​/{max}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `min` | `float` | **Required**. New minimum limit of organic_carbon​ |
| `max` | `float` | **Required**. New maximum limit of organic_carbon​ |

#### Update Organic_carbon​ limits

```http
  POST ​/Limits/Turbidity/{min}/{max}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `min` | `float` | **Required**. New minimum limit of turbidity |
| `max` | `float` | **Required**. New maximum limit of turbidity |

### Restart limits to the default values
```http
  GET ​/Limits​/restartLimits
```







### [User interface](http://localhost:5000/)

## InfluxDB

Pokrenuti [InfluxDB UI](http://localhost:8086/signin) i ulogovati se :

username: **admin**

password: **admin123**

organization: **slavko**

bucket: **slavko**

###

Pritisnuti **load data**, **NodeJS** kopirati token u ```Projekat III /App/visualization``` **app.js** na odgovarajuce mesto

### Dashboards

Create dashboards, **new dashboard**, **add cell**

From **slavko**, **host:edgeX** ***submit***
## Grafana

Pokrenuti [Grafana UI](http://localhost:4200/) i ulogovati se :

username: **admin**

password: **admin**



### Add new source

Name: **InfluxDB**

Query Language: **Flux**

URL: **http://host.docker.internal:8086**

Auth: **Basic auth**

User: **admin**

Password: **admin123**

Organization: **slavko**

Token: **iskopirati token iz InfluxDB **

Default Buclet: **slavko**

### Add new dashboard, new panel

Kopirati flux skriptu iz influxDB script editor, apply
