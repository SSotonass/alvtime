# Using database migrations for Alvtime

The projects found in this solution run migration and population scripts both for developing Alvtime locally and updating the databases in the test and production environments. The solution is split into two projects, one for migrations and one for populations. The migration scripts are used for creating and updating the database both locally and in the environments, while the population script is only used for populating your local database with test data.

## Using locally with Docker

The migration and population scripts can be run a few different ways. In all commands you can omit the `--build` argument if no changes have been made to the code from the last time you ran them. The commands must be run from the root folder of alvtime.
- When running the backend api using the docker command `docker-compose up --build api`, both the migration and population scripts are run to populate your local database running in the docker container.
- If you wish to only run the database scripts using docker, you can use the command `docker-compose up --build db-populator`. This will create the database and run migrations and population.

## Using locally without Docker

If you wish to run the scripts to create and populate an Alvtime database on your local machine and not run it in a Docker container, you can run the projects manually from either Visual Studio, Rider or using the dotnet CLI. 

First you will need to add the connection string to the database to your user secrets by following the guide in the section below called `Add user secrets`. You can the run the projects, starting with `Alvtime.Database.Migrations` to create the database, then `Alvtime.Database.Populations` to populate the test data.

## Add user secrets
Before running locally, please add the following json to your user secrets:
```
{
	"ConnectionString": "<your secret connection string>"
}
```

An example connection string for a local database would look like this:

 ```
 Data Source=localhost\SQLEXPRESS;Initial Catalog=AlvDevDB;Integrated Security=SSPI;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False
 ```

User secrets can be found by right clicking at the project in Visual Studio and select "Manage User Secrets". User secrets can also be added using the dotnet CLI.

## Adding new scripts

To add a new migration or population script, create a new `.sql` file in the correct `Scripts`-folder and prefix it with `ScriptXXXX` following the numbering convention. Before running the project or deploying the changes, right click the newly created script, select `Properties` and set the `Build action` dropdown to `EmbeddedResource`. This is required for the script to be included in the build.