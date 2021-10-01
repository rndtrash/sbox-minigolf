using Sandbox;

namespace Minigolf
{
	public partial class SpectatorComponent : EntityComponent
	{
		[Net] public Ball Spectating { get; set; }
	}
}
