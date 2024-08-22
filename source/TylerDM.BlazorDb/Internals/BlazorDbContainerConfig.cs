namespace TylerDM.BlazorDb.Internals;

public record BlazorDbContainerConfig<TDocument, TId>(
	BlazorDb Database,
	string ContainerName,
	Func<TDocument, TId> GetIdFunc
);
