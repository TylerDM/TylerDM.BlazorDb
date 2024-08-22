namespace TylerDM.BlazorDb;

public static class Startup
{
	public static void AddBlazorDbTesting(
		this IServiceCollection services,
		Action<BlazorDbConfigBuilder> configure,
		Action<LocalStorageOptions>? configureStorage = null,
		string databaseName = ""
	)
	{
		services.AddBlazorDb(configure, configureStorage, databaseName);
		services.switchToTestingLocalStorage(configureStorage);
	}

	private static void switchToTestingLocalStorage(this IServiceCollection services, Action<LocalStorageOptions>? configureStorage = null)
	{
		services.Remove(services.Single(x => x.ServiceType == typeof(ILocalStorageService)));
		var storage = new TestContext().AddBlazoredLocalStorage(configureStorage);
		services.AddSingleton(storage);
	}
}
