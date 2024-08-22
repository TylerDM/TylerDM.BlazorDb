namespace TylerDM.BlazorDb.Internals;

public class BlazorDbConfigBuilder(IServiceCollection _services, BlazorDb _db)
{
	public void AddContainer<TDocument, TId>(Func<TDocument, TId> getId, string name = "")
		where TDocument : class
		where TId : struct
	{
		if (string.IsNullOrWhiteSpace(name))
			name = typeof(TDocument).FullName!;

		var config = new BlazorDbContainerConfig<TDocument, TId>(_db, name, getId);
		_services.AddSingleton<BlazorDbContainer<TDocument, TId>>(x =>
		{
			var storage = x.GetRequiredService<ILocalStorageService>();
			return new(storage, config);
		});
	}
}
