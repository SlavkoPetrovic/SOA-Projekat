
# [SOA - Projekat 1](https://cs.elfak.ni.ac.rs/nastava/pluginfile.php/38764/mod_resource/content/5/SOA%20-%20Projekat%201%20-%202022.pdf)

Informacije o Marvelovim filmovima i njihove ocene

[Dataset ](https://www.kaggle.com/datasets/minisam/marvel-movie-dataset?select=marvel_clean.csv) koji je koriscen

[IMDb API](https://imdb-api.com/) - public api koji je koriscen

## Pokretanje

Pozicionirati se u folderu Projekat/Application

```bash
 cd Projekat/Application
```
Startovati aplikaciju komandom
```bash
 docker compose up --build
```
Za popunjavanje baze podataka pozicionirati se u folderu helper app
```bash
 cd helper_app
```
I pokrenuti aplikaciju komandom
```bash
dotnet run
```
   
## API Reference

[SwaggerUI ](http://localhost:5001/swagger/index.html)

#### Delete movie with title

```http
  DELETE /Gateway/deleteMovie/{title}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `title` | `string` | **Required**. Movie title |

#### Delete all movies

```http
  DELETE /Gateway/deleteMovies
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `string` | **Required**. Id of item to fetch |

#### Post new movie

```http
  POST /Gateway/postMovie
```

| JSON Body  | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `title`      | `string` | **Required**. Movie title |
| `distributor`      | `string` | **Required**. Movie distributor |
| `releaseDateUS`      | `date` | **Required**. Movie releaseDateUS |
| `budget`      | `int` | **Required**. Movie budget |
| `openingWeekendNorthAmerica`      | `int` | **Required**. Opening weekend North America revenue |
| `northAmerica`      | `int` | **Required**. North America revenue |
| `otherTerritories`      | `int` | **Required**. Other countries revenue |
| `worldwide`      | `int` | **Required**. Worldwide revenue |

### Update movie
```http
  PUT ​/Gateway​/updateMovie​/{title}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `title`      | `string` | **Required**. Movie title |

| JSON body | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `title`      | `string` | **Required**. Movie title |
| `distributor`      | `string` | **Required**. Movie distributor |

### Get movie with title
```http
  GET ​/Gateway/getMovie/{title}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `title`      | `string` | **Required**. Movie title |

### Get random movie 

```http
​GET /Gateway​/getRandomMovie
```




# [SOA - Projekat 2](https://cs.elfak.ni.ac.rs/nastava/pluginfile.php/43852/mod_resource/content/9/SOA%20-%20Projekat%202%20-%202022.pdf)

Prosirenje Projekta I, izmenjen gateway, dodan Mosquitto MQTT publish

## Pokretanje

Ukoliko je pravilno pokrenut Projekat I preskociti ovu sekciju

Pozicionirati se u folderu Projekat/Application

```bash
 cd Projekat/Application
```
Startovati aplikaciju komandom
```bash
 docker compose up --build
```
Za popunjavanje baze podataka pozicionirati se u folderu helper app
```bash
 cd helper_app
```
I pokrenuti aplikaciju komandom
```bash
docker run
```
   
## eKuiper

Pokrenuti [eKuiper manager](http://localhost:9082) i ulogovati se :

**username:** admin

**password:** public

### Dodati novi servis

**Service type:** Direct link service

**Service name:** movie

**Endpoint:** ```http://host.docker.internal:9081```

### Dodati novi stream
**Stream name:** Direct link service

**Cekirati Whether the schema stream**

### Add stream fields: 

Title: **string**

Distributor: **string**

Budget: **bigint**

OpeningWeekendNorthAmerica: **bigint**

NorthAmerica: **bigint**

OtherTerritories: **bigint**

Worldwide: **bigint**

Endpoint: ```http://host.docker.internal:9081```

### 

datasource: **inputMQTT**

format: **json**

confKey: **default**

type: **mqtt**

strictValidation: **true**

### Rules

Rule ID: **newRule**

Name: **new Rule**

SQL: ```select * from movies where Worldwide > 629054379```

### Actions 

Sink: **mqtt**

MQTT broker address: ```tcp://broker.emqx.io:1883```

MQTT topic: **outputMQTT**

Stream Format: **json**

## InfluxDB

Pokrenuti [InfluxDB UI](http://localhost:8086/signin) i ulogovati se :

username: **admin**

password: **admin123**

organization: **slavko**

bucket: **slavko**

###

Pritisnuti **load data**, **NodeJS** kopirati token u ```Projekat/Application/analytics``` **app.js** na odgovarajuce mesto

### Dashboards

Create dashboards, **new dashboard**, **add cell**

From **slavko**, **host:host1** ***submit***