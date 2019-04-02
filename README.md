# Web crawler

The crawler should run on any system supported by .NET Core: Windows, Ubuntu, Mac but is tested only on Windows.
1. .NET Core Framework 2.2 must be installed
2. Set the database connection string in appsettings.json as:
	"Host=URL;Port=PRT;Database=DBN;Username=USR;Password=PWD"
	where URL, PRT, DBN, USR, PWD are server URL, port, database name, username and password respectively.
3. Run the project WebCrawler.csproj:
	- with Visual Studio
	- or with .NET CLI "dotnet run WebCrawler.csproj"

