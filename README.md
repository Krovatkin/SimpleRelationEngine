
## Testing

1. For testing, create a new project w/: `dotnet new xunit -o tests`
2. Add the reference to the main project: `dotnet add reference ../src/sqlexec2.csproj`
    * Note, the main project can't be in the parent folder!
3. Run tests: `cd tests; dotnet test`

A good full example of using xunit can be found [here](https://learn.microsoft.com/en-us/dotnet/core/tutorials/testing-with-cli)

