namespace TylerDM.BlazorDb;

public static class Startup
{
	public static void AddBlazorDb(
		this IServiceCollection services,
		Action<BlazorDbConfigBuilder> configureDb,
		Action<LocalStorageOptions>? configureStorage = null,
		string databaseName = ""
	)
	{
		if (string.IsNullOrWhiteSpace(databaseName))
			databaseName = nameof(BlazorDb);

		var database = new BlazorDb(databaseName);

		services.AddBlazoredLocalStorageAsSingleton(configureStorage);

		var builder = new BlazorDbConfigBuilder(services, database);
		configureDb(builder);
	}
}
