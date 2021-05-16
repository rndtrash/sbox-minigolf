using Sandbox;

namespace Minigolf
{
	[Library("minigolf_speed_boost")]
	public partial class SpeedBoost : ModelEntity
	{
		[HammerProp( "SpeedMultiplier" )]
		public float SpeedMultiplier { get; set; }

		[HammerProp( "MaxSpeed" )]
		public float MaxSpeed { get; set; }

		[HammerProp( "movedir" )]
		public Angles MoveDir { get; set; }

		[HammerProp( "movedir_islocal" )]
		public bool MoveDirIsLocal { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			EnableDrawing = true;

			SetupPhysicsFromModel( PhysicsMotionType.Static );

			// Custom surface property with 0 friction, 0 elasticity.
			// PhysicsGroup.SetSurface( "minigolf.side" );

			Transmit = TransmitType.Always;
		}
	}
}
