namespace TylerDM.BlazorDb;

public record Document(
	Guid Id
)
{
	public int X { get; set; } = NextInt();
	public int Y { get; set; } = NextInt();
}
