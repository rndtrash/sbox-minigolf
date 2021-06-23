using Sandbox;
using Sandbox.Internal;

namespace Minigolf
{
	[Library("minigolf_hole_goal", Description = "Minigolf Hole" )]
	[Hammer.Solid]
	public partial class HoleGoal : ModelEntity
	{
		[Property( "hole_number", "Hole Number", "Which hole this hole is on." )]
		public int Hole { get; set; } = 1;

		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel(PhysicsMotionType.Static);
			CollisionGroup = CollisionGroup.Trigger;
			EnableSolidCollisions = false;
			EnableTouch = true;
		}

		public override void StartTouch( Entity other )
		{
			if ( other is not GolfBall ball )
				return;

			(Game.Current as GolfGame).CupBall( ball, Hole );
		}
	}
}
