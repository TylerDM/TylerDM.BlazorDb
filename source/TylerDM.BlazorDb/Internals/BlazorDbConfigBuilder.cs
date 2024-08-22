namespace TylerDM.BlazorDb.Internals;

public class BlazorDbConfigBuilder
{
	public string DatabaseName { get; set; } = "";

	public void AddContainer<TDocument, TId>(Func<TDocument, TId> getId, string key = "")
		where TDocument : class
		where TId : struct
	{
		
	}

	public BlazorDbConfig Build()
	{
		var frozenKeys = _keys.ToFrozenDictionary();
		var frozenGetIdFunctions = _getIdFuncs.ToFrozenDictionary();
		return new(frozenKeys, frozenGetIdFunctions, DatabaseName);
	}
}
