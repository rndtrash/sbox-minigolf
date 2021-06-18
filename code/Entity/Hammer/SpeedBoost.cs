using Sandbox;

namespace Minigolf
{
	[Library( "minigolf_speed_boost", Description = "An brush built entity that will boost balls." )]
	[Hammer.DrawAngles( "movedir", "movedir_is_local" )]
	public partial class SpeedBoost : ModelEntity
	{
		[Property( "SpeedMultiplier", "Speed Multiplier", "How much the ball will accelerate" )]
		public float SpeedMultiplier { get; set; } = 2.0f;

		[Property( "MaxSpeed", "Max Speed", "Max speed the ball can be accelerated to from this booster." )]
		public float MaxSpeed { get; set; } = 1000.0f;

		[Property( "movedir", "Move Direction (Pitch Yaw Roll)", "The direction the ball will move, when told to." )]
		public Angles MoveDir { get; set; }

		[Property( "movedir_islocal", "Move Direction is Expressed in Local Space", "If checked, the movement direction angle is in local space and should be rotated by the entity's angles after spawning." )]
		public bool MoveDirIsLocal { get; set; } = true;

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
