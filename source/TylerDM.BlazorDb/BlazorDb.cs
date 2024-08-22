namespace TylerDM.BlazorDb;

public class BlazorDb(BlazorDbConfig _config)
{
	public string Name => _config.Name;
}
