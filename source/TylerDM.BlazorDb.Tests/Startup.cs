namespace TylerDM.BlazorDb;

public class Startup
{
	public void ConfigureHost(IHostBuilder hostBuilder)
	{
	}

	public void ConfigureServices(IServiceCollection services)
	{
		var localStorageService = getTestLocalStorage();
		services.AddBlazorLocalStorageDb(builder =>
		{
			builder.AddTable((Table item) => item.Id);
		}, localStorageService);
	}

	public void Configure()
	{
	}

	private ILocalStorageService getTestLocalStorage() =>
		new TestContext().AddBlazoredLocalStorage();
}
