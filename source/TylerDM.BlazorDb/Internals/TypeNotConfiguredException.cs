namespace TylerDM.BlazorDb.Internals;

internal class TypeNotConfiguredException(Type type) : InvalidOperationException(_message)
{
	private const string _message = "Type was not configured with the DB during startup.";

	public Type Type { get; } = type;
}
