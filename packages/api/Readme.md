# AlvTime-WebApi
## Back-end of alvtime

### Run locally
Navigate to root folder and run `docker-compose up --build api`. The API is available at `http://localhost:8081`. To call endpoints with the `[Authorize]`-tag an access token is needed. Use the following token `5801gj90-jf39-5j30-fjk3-480fj39kl409` for non-admin endpoints.
For admin endpoints you need an admin user token, which can be extracted from logging into frontend and viewing console. If you are not an Alv user you will need to either change the authorization provider in `appsettings.json` or remove the `[Authorize]`-tag. 

### Run locally without Docker
In order to use the API in a meaningful way you will need to setup a local database. Follow the user guide found in `alvtime/packages/db/README.md` for local setup to create a local database with test data. Add the connection string to your local database to your user secrets in the `AlvTimeWebApi` project by following the guide in the section below. Now start the API through Visual Studio or Rider and start making requests using the token found in the section above.

## Add user secrets
Before running locally, please add the following json to your user secrets:
```
{
    "ConnectionStrings": {
        "AlvTime_db": "<your secret connection string>"
    }
}
```
An example connection string for a local database would look like this:

 ```
 Data Source=localhost\SQLEXPRESS;Initial Catalog=AlvDevDB;Integrated Security=SSPI;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False
 ```

User secrets can be found by right clicking at the project in Visual Studio and select "Manage User Secrets". User secrets can also be added using the dotnet CLI.