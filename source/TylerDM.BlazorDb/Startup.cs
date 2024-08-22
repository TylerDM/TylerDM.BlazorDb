namespace TylerDM.BlazorDb;

public static class Startup
{
	public static void AddBlazorLocalStorageDb(this IServiceCollection services, Action<BlazorDbConfigBuilder> configureDb, Action<LocalStorageOptions>? configureStorage = null)
	{
		services.AddBlazoredLocalStorage(configureStorage);
		services.addBlazorLocalStorageDb(configureDb);
	}

	/// <summary>
	/// Adds the LocalStorageDb with a specified ILocalStorageService.  Usually only useful if you are testing.
	/// </summary>
	public static void AddBlazorLocalStorageDb(this IServiceCollection services, Action<BlazorDbConfigBuilder> configureDb, ILocalStorageService localStorageService)
	{
		services.AddSingleton(localStorageService);
		services.addBlazorLocalStorageDb(configureDb);
	}

	private static void addBlazorLocalStorageDb(this IServiceCollection services, Action<BlazorDbConfigBuilder> configureDb)
	{
		services.AddSingleton<BlazorDb>();
		services.AddSingleton(typeof(BlazorDbContainer<>));

		var builder = new BlazorDbConfigBuilder();
		configureDb(builder);
		var config = builder.Build();
		services.AddSingleton(config);
	}
}
