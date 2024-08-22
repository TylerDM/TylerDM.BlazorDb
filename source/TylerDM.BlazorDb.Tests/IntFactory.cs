namespace TylerDM.BlazorDb;

public class IntFactory
{
	private int value = 0;

	public int NextInt() =>
		Interlocked.Increment(ref value);
}
