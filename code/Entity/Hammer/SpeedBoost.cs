using Sandbox;
using Sandbox.Internal;

namespace Minigolf
{
	[Library( "minigolf_speed_boost", Description = "An brush built entity that will boost balls." )]
	[Hammer.Solid]
	[Hammer.DrawAngles( "movedir" )]
	[Hammer.RenderProperties]
	[Hammer.PhysicsTypeOverride(Hammer.PhysicsTypeOverrideAttribute.PhysicsTypeOverride.Mesh)]
	public partial class SpeedBoost : ModelEntity
	{
		[Property( "SpeedMultiplier", "Speed Multiplier", "How much the ball will accelerate" )]
		public float SpeedMultiplier { get; set; } = 2.0f;

		[Property( "MaxSpeed", "Max Speed", "Max speed the ball can be accelerated to from this booster." )]
		public float MaxSpeed { get; set; } = 1000.0f;

		[Property( "movedir", "Move Direction (Pitch Yaw Roll)", "The direction the ball will move, when told to." )]
		public Angles MoveDir { get; set; }

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
