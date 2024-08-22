namespace TylerDM.BlazorDb;

public class Startup
{
	public void ConfigureHost(IHostBuilder hostBuilder)
	{
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddBlazorDbTesting(builder =>
		{
			builder.AddContainer<Document, int>(x => x.Id);
		});
	}

	public void Configure()
	{
	}
}
