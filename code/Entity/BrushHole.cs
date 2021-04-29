using Sandbox;

namespace Minigolf
{
	[Library("minigolf_brush_hole")]
	public partial class BrushHole : ModelEntity
	{
		[HammerProp("hole")]
		public int Hole { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel(PhysicsMotionType.Static);
			CollisionGroup = CollisionGroup.Trigger;
			EnableSolidCollisions = false;
			EnableTouch = true;

			Transmit = TransmitType.Never;
		}

		public override void StartTouch(Entity other)
		{
			if (other is PlayerBall)
				(Game.Current as GolfGame).OnBallInHole(other as PlayerBall, Hole);
		}
	}
}
