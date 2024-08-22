namespace TylerDM.BlazorDb;

public static class Functions
{
	private static readonly IntFactory _intFactory = new();

	public static int NextInt() =>
		_intFactory.NextInt();

	public static Task AssertThrowsAsync(Func<ValueTask<Table>> func) =>
		Assert.ThrowsAnyAsync<Exception>(async () => await func());

	public static Task AssertThrowsAsync(Func<ValueTask> func) =>
		Assert.ThrowsAnyAsync<Exception>(async () => await func());
}
