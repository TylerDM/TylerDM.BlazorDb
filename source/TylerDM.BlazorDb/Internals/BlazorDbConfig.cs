namespace TylerDM.BlazorDb.Internals;

public record BlazorDbConfig(
	FrozenDictionary<Type, string> KeyPrefixes,
	FrozenDictionary<Type, Func<object, object>> GetIdFunctions
);
