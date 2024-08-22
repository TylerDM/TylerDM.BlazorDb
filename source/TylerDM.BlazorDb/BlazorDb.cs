namespace TylerDM.BlazorDb;

public class BlazorDb<TItem>(LocalStorageDb _db)
	where TItem : class
{
	public ValueTask UpdateAsync(TItem item) =>
		_db.UpdateAsync(item);

	public ValueTask AddAsync(TItem item) =>
		_db.AddAsync(item);

	public ValueTask AddOrUpdateAsync(TItem item) =>
		_db.AddOrUpdateAsync(item);

	public ValueTask<bool> ExistsAsync<TId>(TId id)
		where TId : struct =>
		_db.ExistsAsync<TItem, TId>(id);

	public ValueTask<bool> ExistsAsync(Func<TItem, bool> predicate) =>
		_db.ExistsAsync(predicate);

	public ValueTask<bool> ExistsAsync(TItem item) =>
		_db.ExistsAsync(item);

	public IAsyncEnumerable<TItem> QueryAsync() =>
		_db.QueryAsync<TItem>();

	public ValueTask<TItem?> GetAsync(Func<TItem, bool> predicate) =>
		_db.GetAsync(predicate);

	public ValueTask<TItem> GetRequiredAsync(Func<TItem, bool> predicate) =>
		_db.GetRequiredAsync(predicate);

	public ValueTask<List<TItem>> GetAllAsync(Func<TItem, bool> predicate) =>
		_db.GetAllAsync(predicate);

	public ValueTask<TItem> GetRequiredAsync<TId>(TId id)
		where TId : struct =>
		_db.GetRequiredAsync<TItem, TId>(id);

	public ValueTask DeleteAsync<TId>(TId id)
		where TId : struct =>
		_db.DeleteAsync<TItem, TId>(id);

	public ValueTask DeleteAsync(TItem item) =>
		_db.DeleteAsync(item);
}

public class LocalStorageDb(ILocalStorageService _storage, BlazorDbConfig _config) : BlazorDbBase(_storage, _config)
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
}
