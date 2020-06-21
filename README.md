> **⚠ WIP. Ich entwickelte den Bot um 3 Uhr nachts aus Langeweile. Momentan noch "quick and dirty", aber er funktioniert als POC**.

![.NET Core](https://github.com/ginomessmer/preis-alarm/workflows/.NET%20Core/badge.svg)
![Docker Image CI](https://github.com/ginomessmer/preis-alarm/workflows/Docker%20Image%20CI/badge.svg)
[![](https://img.shields.io/docker/cloud/automated/ginomessmer/preis-alarm)](https://hub.docker.com/r/ginomessmer/preis-alarm)

# Preis Alarm
Der Preis ist heiß. Ein Discord-Bot, der deine Lieblingsangebote in deinem Edeka um die Ecke ausspuckt.

### Unterstützte Märkte
- [x] Edeka

---

## Konfiguration
- `ConnectionStrings:DiscordBotToken`: Discord Bot Token (https://discord.com/developers/)

## Deployment
### Docker Run
```sh
docker run -v ./data.db:/app/data.db -e "ConnectionStrings:DiscordBotToken=${DiscordToken}" --restart always -d ginomessmer/preis-alarm
```

### Docker Compose
```yml
version: '3.8'

services:
  instance:
    image: ginomessmer/preis-alarm
    volumes:
      - "./data.db:/app/data.db"
    environment:
      - "ConnectionStrings:DiscordBotToken=${DiscordToken}"
```

## Commands
- `€edeka deals`: Gibt die aktuellen Angebote zurück
- `€edeka markets <suchbegriff>`: Sucht Märkte anhand des Begriffes und listet ihre IDs auf.
- `€set marketId`: Setzt die Edeka Markt ID
- `€kw`: Listet alle Lieblingsstichworte
- `€kw+ <stichwort1> [<stichwort2> ...]`: Fügt ein neues Stichwort hinzu (bspw. "Nüsse")
- `€kw- <stichwort1> [<stichwort2> ...]`: Entfernt Stichworte

![](https://i.imgur.com/mXoFVdU.png)
