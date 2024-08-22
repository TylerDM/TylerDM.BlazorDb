namespace TylerDM.BlazorDb.Internals;

public record BlazorDbContainerConfig<TDocument, TId>(
	string Name,
	Func<TDocument, TId> GetIdFunc,
	BlazorDbConfig DbConfig
);
