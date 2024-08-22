namespace TylerDM.BlazorDb;

public record Table(
	Guid Id
)
{
	public int X { get; set; } = NextInt();
	public int Y { get; set; } = NextInt();
}
