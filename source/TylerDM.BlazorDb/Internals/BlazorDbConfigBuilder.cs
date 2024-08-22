namespace TylerDM.BlazorDb.Internals;

public class BlazorDbConfigBuilder
{
	private readonly Dictionary<Type, string> _keys = [];
	private readonly Dictionary<Type, Func<object, object>> _getIdFuncs = [];

	public void AddTable<TItem, TId>(Func<TItem, TId> getId, string key = "")
		where TItem : class
		where TId : struct
	{
		object getIdFunc(object obj) => getId((TItem)obj);

		var type = typeof(TItem);
		if (key.Length == 0)
			key = type.FullName ?? type.ToString();

		_keys.Add(type, key);
		_getIdFuncs.Add(type, getIdFunc);
	}

	public BlazorDbConfig Build()
	{
		var frozenKeys = _keys.ToFrozenDictionary();
		var frozenGetIdFunctions = _getIdFuncs.ToFrozenDictionary();
		return new(frozenKeys, frozenGetIdFunctions);
	}
}
