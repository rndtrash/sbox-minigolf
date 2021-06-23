using Sandbox;
using Sandbox.Internal;

namespace Minigolf
{
	[Library("minigolf_hole_spawn", Description = "Minigolf Ball Spawn" )]
	[Hammer.EditorModel( "models/golf_ball.vmdl" )]
	[Hammer.DrawAngles]
	public partial class HoleSpawn : Entity
	{
		[Property( "hole_number", "Hole Number", "Which hole this spawnpoint is on." )]
		public int Number { get; set; } = 1;

		[Property( "hole_name", "Hole Name", "cool name" )]
		public string Name { get; set; } = "wanker";

		[Property( "hole_par", "Hole Par", "How many strokes should this hole be done in." )]
		public int Par { get; set; } = 3;
	}
}
