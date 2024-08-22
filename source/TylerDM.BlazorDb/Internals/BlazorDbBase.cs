namespace TylerDM.BlazorDb.Internals;

public abstract class BlazorDbBase
{
	protected readonly ILocalStorageService _storage;
	protected readonly BlazorDbConfig _config;

	internal BlazorDbBase(ILocalStorageService storage, BlazorDbConfig config)
	{
		_storage = storage;
		_config = config;
	}

	protected string getKey<TItem, TId>(TId id)
		where TItem : class
		where TId : struct =>
		$"{getPrefix<TItem>()}_{id}";

	protected string getKey<T>(T item)
		where T : class =>
		$"{getPrefix<T>()}_{getId(item)}";

	protected string getId<T>(T item)
		where T : class
	{
		var type = typeof(T);
		if (_config.GetIdFunctions.TryGetValue(type, out var func))
			return func(item).ToString() ??
				throw new Exception("Get ID function returned null for the given item.");
		throw new TypeNotConfiguredException(type);
	}

	protected string getPrefix<T>()
	{
		var type = typeof(T);
		if (_config.KeyPrefixes.TryGetValue(type, out var prefix)) return prefix;
		throw new TypeNotConfiguredException(type);
	}
}
