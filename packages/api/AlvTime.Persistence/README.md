## Updating database

To make changes to the database, for example create a new table or populate additional test data for local development, follow the user guide found in `alvtime/packages/db/README.md`.

## Scaffolding
To update database models after changes in the database schema, first install the `dotnet ef` tool by running the following command: `dotnet tool install --global dotnet-ef`. You will need a dotnet SDK installed on your computer for this. Next run the following command in the AlvTime.Persistence directory:

`dotnet ef dbcontext scaffold "Server=.\;Database=AlvDevDB;Trusted_Connection=False;User ID=sa;Password=AlvTimeTestErMoro32" Microsoft.EntityFrameworkCore.SqlServer -o DatabaseModels -c "AlvTime_dbContext" --no-pluralize --force`

If you have made the changes in your local Docker database you can use the following server:
`localhost,1434`.