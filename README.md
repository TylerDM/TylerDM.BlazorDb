# Intro
A wrapper around https://github.com/Blazored/LocalStorage that creates an extremely simple document database.

## Setup
To set up the library, add it to your DI container.  Don't forget to add your entity types ahead of time.
```csharp
services.AddBlazorLocalStorageDb(builder =>
{
	builder.AddTable<Person, Guid>(person => person.Id);
});
```

`AddBlazorDb()` also has an overload for customizing the underlying `ILocalStorageService`.

## Usage
Inject `LocalStorageDb` into your components or services and use any of the available methods.
```csharp
class PersonFactory(BlazorDb _db)
{
	public async Task<Person> CreateAsync(string firstName, string lastName)
	{
		var person = new Person(firstName, lastName);
		await _db.AddAsync(person);
		return person;
	}
}

class PersonRepository(BlazorDb _db)
{
	public async Task<List<Person>> GetJohns() =>
		await _db.GetAllAsync(x => x.FirstName == "John");
}
```