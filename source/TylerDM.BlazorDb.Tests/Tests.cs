namespace TylerDM.BlazorDb;

public class Tests(BlazorDbContainer<Document> _db)
{
	[Fact]
	public async Task GetAsync()
	{
		var item = await createAndAddRandomAsync();
		Assert.NotNull(await _db.GetAsync(x => x.Id == item.Id));
		Assert.Null(await _db.GetAsync(x => x.Id == Guid.NewGuid()));
	}

	[Fact]
	public async Task ExistsAsync()
	{
		var exists = await createAndAddRandomAsync();
		var doesNotExist = createRandom();

		//ID
		Assert.True(await _db.ExistsAsync(exists.Id));
		Assert.False(await _db.ExistsAsync(doesNotExist.Id));

		//Item
		Assert.True(await _db.ExistsAsync(exists));
		Assert.False(await _db.ExistsAsync(doesNotExist));

		//Predicate
		Assert.True(await _db.ExistsAsync(x => x.Id == exists.Id));
		Assert.False(await _db.ExistsAsync(x => x.Id == doesNotExist.Id));
	}

	[Fact]
	public async Task GetRequiredAsync()
	{
		Task assertThrowsAsync(Func<ValueTask<Document>> func) =>
				Assert.ThrowsAnyAsync<Exception>(async () => await func());

		var item = await createAndAddRandomAsync(default, default);

		//ID
		await _db.GetRequiredAsync(item.Id);
		await assertThrowsAsync(() => _db.GetRequiredAsync(0));

		//Predicate
		await _db.GetRequiredAsync(x => x.Id == item.Id);
		await assertThrowsAsync(() => _db.GetRequiredAsync(x => false));
	}

	[Fact]
	public async Task GetAllAsync()
	{
		var testX = NextInt();
		var matchingY = NextInt();
		var nonMatchingY = NextInt();

		async ValueTask addMatchingAsync() =>
			await createAndAddRandomAsync(testX, matchingY);
		async ValueTask addNonmatchingAsync() =>
			await createAndAddRandomAsync(testX, nonMatchingY);

		await addMatchingAsync();
		await addMatchingAsync();
		await addMatchingAsync();
		await addNonmatchingAsync();
		await addNonmatchingAsync();

		var totalElements = await _db.GetAllAsync(x => x.X == testX);
		Assert.Equal(5, totalElements.Count);

		var matchingElements = await _db.GetAllAsync(x => x.Y == matchingY);
		Assert.Equal(3, matchingElements.Count);

		var nonexistent = await _db.GetAllAsync(x => x.Id == Guid.NewGuid());
		Assert.Empty(nonexistent);
	}

	[Fact]
	public async Task DeleteAsync()
	{
		{
			//Test by ID
			var id = await createAndAddRandomAsync();
			Assert.True(await _db.ExistsAsync(id));
			await _db.DeleteAsync(id);
			Assert.False(await _db.ExistsAsync(id));
		}

		{
			//Test by value
			var item = await createAndAddRandomAsync();
			Assert.True(await _db.ExistsAsync(item));
			await _db.DeleteAsync(item);
			Assert.False(await _db.ExistsAsync(item));
		}
	}

	[Fact]
	public async Task AddOrUpdateAsync()
	{
		var item = await createAndAddRandomAsync();
		var restored = await _db.GetRequiredAsync(item.Id);
		Assert.Equal(item, restored);

		item.X = NextInt();
		await _db.UpdateAsync(item);
		restored = await _db.GetRequiredAsync(item.Id);
		Assert.Equal(item, restored);
	}

	[Fact]
	public async Task AddAsync()
	{
		var item = await createAndAddRandomAsync();

		//Throw if you try to add a duplicate.
		await AssertThrowsAsync(() => _db.AddAsync(item));

		var restored = await _db.GetRequiredAsync(item.Id);
		Assert.Equal(item, restored);
	}

	[Fact]
	public async Task UpdateAsync()
	{
		var item = createRandom();
		Document restored;

		//Throw if doesn't exist.
		await AssertThrowsAsync(() => _db.UpdateAsync(item));

		//Call update to see if throws and verify value is still correct.
		await _db.AddAsync(item);
		await _db.UpdateAsync(item);
		restored = await _db.GetRequiredAsync(item.Id);
		Assert.Equal(item, restored);

		//Change the original item and verify the update works.
		item.X = 5;
		await _db.UpdateAsync(item);
		restored = await _db.GetRequiredAsync(item.Id);
		Assert.Equal(item, restored);
	}

	static Document createRandom(int? x = null, int? y = null) =>
		new(Guid.NewGuid())
		{
			X = x ?? NextInt(),
			Y = y ?? NextInt()
		};

	async ValueTask<Document> createAndAddRandomAsync(int? x = null, int? y = null)
	{
		var document = createRandom(x, y);
		await _db.AddOrUpdateAsync(document);
		return document;
	}
}
