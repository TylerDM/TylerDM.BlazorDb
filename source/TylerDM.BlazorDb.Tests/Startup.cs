namespace TylerDM.BlazorDb;

public class Startup
{
	public void ConfigureHost(IHostBuilder hostBuilder)
	{
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddBlazorDb(builder =>
		{
			builder.AddContainer((Document item) => item.Id);
		});
		switchToTestingLocalStorage(services);
	}

	public void Configure()
	{
	}

	private void switchToTestingLocalStorage(IServiceCollection services)
	{
		services.Remove(services.Single(x => x.ServiceType == typeof(ILocalStorageService)));
		services.AddSingleton(new TestContext().AddBlazoredLocalStorage());
	}
}
