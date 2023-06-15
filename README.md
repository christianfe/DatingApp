
# DatingApp

The projects consists in a DatingApp



## Demo

https://datingapp.christianfelicione.it


## Environment Variables

To run this project, you will need to create the file to `.\API\appsettings.json` file

````json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
   }
  },
  "ConnectionStrings": {
    "DefaultConnection": "CONNECTION STRING FOR CONNECTION TO POSTGRESQL"
  },
  "AllowedHosts": "*",
  "ClaudinarySettings": {
    "CloudName": "!!CLOUDINARY CLOUD NAME!!",
    "ApiKey": "!!CLOUDINARY API KEY!!",
    "ApiSecret": "!!CLOUDINARY API SECRET!!"
  },
  "TokenKey": "!!CREATE_SECRET_KEY_HERE!!"
}
````



## Authors

- [@christianfe](https://www.github.com/christianfe)


## Tech Stack

**Client:** Angular, Bootstrap

**Server:** .Net core, Entity Framework, SQLite, PostgreSQL

