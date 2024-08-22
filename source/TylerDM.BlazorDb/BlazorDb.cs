namespace TylerDM.BlazorDb;

public class BlazorDb(ILocalStorageService _storage, BlazorDbConfig _config)
{
	public async ValueTask UpdateAsync<T>(T item)
		where T : class
	{
		if (await ExistsAsync(item) == false) throw new Exception("Item with type and ID does not exist in DB.");
		await AddOrUpdateAsync(item);
	}

	public async ValueTask AddAsync<T>(T item)
		where T : class
	{
		if (await ExistsAsync(item)) throw new Exception("Item with type and ID already exists in DB.");
		await AddOrUpdateAsync(item);
	}

	public ValueTask AddOrUpdateAsync<T>(T item)
		where T : class
	{
		var key = getKey(item);
		return _storage.SetItemAsync(key, item);
	}

	public ValueTask<bool> ExistsAsync<TItem, TId>(TId id)
		where TItem : class
		where TId : struct
	{
		var key = getKey<TItem, TId>(id);
		return _storage.ContainKeyAsync(key);
	}

	public async ValueTask<bool> ExistsAsync<T>(Func<T, bool> predicate)
		where T : class =>
		await GetAsync(predicate) != null;

	public ValueTask<bool> ExistsAsync<T>(T item)
		where T : class
	{
		var key = getKey(item);
		return _storage.ContainKeyAsync(key);
	}

	public async IAsyncEnumerable<T> QueryAsync<T>()
	{
		var prefix = getPrefix<T>() + '_';
		var keys = await _storage.KeysAsync();
		foreach (var key in keys)
			if (key.StartsWith(prefix))
			{
				var value = await _storage.GetItemAsync<T>(key) ??
					throw new Exception("Key found in local storage but later returned null.");
				yield return value;
			}
	}

	public async ValueTask<T?> GetAsync<T>(Func<T, bool> predicate)
		where T : class
	{
		await foreach (var item in QueryAsync<T>())
			if (predicate(item))
				return item;
		return null;
	}

	public async ValueTask<T> GetRequiredAsync<T>(Func<T, bool> predicate)
		where T : class =>
		await GetAsync(predicate) ??
		throw new Exception("No entity matched predicate.");

	public async ValueTask<List<T>> GetAllAsync<T>(Func<T, bool> predicate)
		where T : class
	{
		var result = new List<T>();
		await foreach (var item in QueryAsync<T>())
			if (predicate(item))
				result.Add(item);
		return result;
	}

	public async ValueTask<TItem> GetRequiredAsync<TItem, TId>(TId id)
		where TItem : class
		where TId : struct
	{
		var key = getKey<TItem, TId>(id);
		return await _storage.GetItemAsync<TItem>(key) ??
			throw new Exception("Item with given ID and Type not found.");
	}

	public ValueTask DeleteAsync<TItem, TId>(TId id)
		where TItem : class
		where TId : struct
	{
		var key = getKey<TItem, TId>(id);
		return _storage.RemoveItemAsync(key);
	}

	public ValueTask DeleteAsync<T>(T item)
		where T : class
	{
		var key = getKey(item);
		return _storage.RemoveItemAsync(key);
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
