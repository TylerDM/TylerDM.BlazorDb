namespace TylerDM.BlazorDb;

public class BlazorDbContainer<TDocument, TId>(
	ILocalStorageService _storage,
	BlazorDbContainerConfig<TDocument, TId> _config
)
	where TDocument : class
	where TId : struct
{
	private readonly string _keyPrefix = $"{_config.Database.Name}_{_config.ContainerName}_";

	public BlazorDb Database => _config.Database;
	public string Name => _config.ContainerName;

	public async ValueTask UpdateAsync(TDocument item)
	{
		if (await ExistsAsync(item) == false)
			throw new Exception("Item with type and ID does not exist in DB.");

		await AddOrUpdateAsync(item);
	}

	public async ValueTask<TDocument> AddAsync(TDocument item)
	{
		if (await ExistsAsync(item))
			throw new Exception("Item with type and ID already exists in DB.");

		await AddOrUpdateAsync(item);
		return item;
	}

	public async ValueTask<TDocument> AddOrUpdateAsync(TDocument item)
	{
		var key = getKey(item);
		await _storage.SetItemAsync(key, item);
		return item;
	}

	public ValueTask<bool> ExistsAsync(TId id)
	{
		var key = getKey(id);
		return _storage.ContainKeyAsync(key);
	}

	public async ValueTask<bool> ExistsAsync(Func<TDocument, bool> predicate) =>
		await GetAsync(predicate) != null;

	public ValueTask<bool> ExistsAsync(TDocument item)
	{
		var key = getKey(item);
		return _storage.ContainKeyAsync(key);
	}

	public async IAsyncEnumerable<TDocument> QueryAsync()
	{
		var keys = await _storage.KeysAsync();
		foreach (var key in keys)
			if (key.StartsWith(_keyPrefix))
			{
				var value = await _storage.GetItemAsync<TDocument>(key) ??
					throw new Exception("Key found in local storage but later returned null.");
				yield return value;
			}
	}

	public async ValueTask<TDocument?> GetAsync(Func<TDocument, bool> predicate)
	{
		await foreach (var item in QueryAsync())
			if (predicate(item))
				return item;
		return null;
	}

	public async ValueTask<TDocument> GetRequiredAsync(Func<TDocument, bool> predicate) =>
		await GetAsync(predicate) ?? throw new Exception("No entity matched predicate.");

	public async ValueTask<List<TDocument>> GetAllAsync(Func<TDocument, bool> predicate)
	{
		var result = new List<TDocument>();
		await foreach (var item in QueryAsync())
			if (predicate(item))
				result.Add(item);
		return result;
	}

	public async ValueTask<TDocument> GetRequiredAsync(TId id)
	{
		var key = getKey(id);
		return await _storage.GetItemAsync<TDocument>(key) ??
			throw new Exception("Item with given ID and Type not found.");
	}

	public ValueTask DeleteAsync(TId id)
	{
		var key = getKey(id);
		return _storage.RemoveItemAsync(key);
	}

	public ValueTask DeleteAsync(TDocument item)
	{
		var key = getKey(item);
		return _storage.RemoveItemAsync(key);
	}

	protected string getKey(TId id) =>
		_keyPrefix + id;

	protected string getKey(TDocument item) =>
		_keyPrefix + getId(item);

	protected TId getId(TDocument item) =>
		_config.GetIdFunc(item);
}
