using Sandbox;

namespace Minigolf
{
	/// <summary>
	/// An brush built entity that will boost balls.
	/// </summary>
	[Library( "minigolf_speed_boost" )]
	[Hammer.Solid]
	[Hammer.DrawAngles( nameof(MoveDir) )]
	[Hammer.PhysicsTypeOverride(Hammer.PhysicsTypeOverrideAttribute.PhysicsTypeOverride.Mesh)]
	public partial class SpeedBoost : ModelEntity
	{
		/// <summary>
		/// How much the ball will accelerate.
		/// </summary>
		[Property]
		public float SpeedMultiplier { get; set; } = 2.0f;

		/// <summary>
		/// Max speed the ball can be accelerated to from this booster.
		/// </summary>
		[Property]
		public float MaxSpeed { get; set; } = 1000.0f;

		/// <summary>
		/// The direction the ball will move, when told to.
		/// </summary>
		[Property( Title = "Move Direction (Pitch Yaw Roll)" )]
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
