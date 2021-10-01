using Sandbox;

namespace Minigolf
{
	public partial class ReadyComponent : EntityComponent
	{
		[Net] public bool Ready { get; set; }
	}
}
