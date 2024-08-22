namespace TylerDM.BlazorDb;

public class BlazorDbContainer<TItem>(BlazorDb _db)
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
