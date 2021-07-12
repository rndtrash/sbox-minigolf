using Sandbox;

namespace Minigolf
{
	public partial class GolfBall : ModelEntity
	{
		[ServerVar( "minigolf_ball_debug" )]
		public static bool Debug { get; set; } = false;

		[Net] public bool Moving { get; set; } = false;
		[Net] public bool Cupped { get; set; } = false;

		static readonly SoundEvent CuppedSound = new SoundEvent( "sounds/minigolf.ball_inhole.vsnd" );

		public GolfBall()
		{
			Camera = new FollowBallCamera( this );
		}

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/golf_ball.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );

			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Debris;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;

			Transmit = TransmitType.Always;
		}

		public void Cup( bool holeInOne )
		{
			if ( Cupped ) return;

			// TODO: Launch the ball on a hole in one

			// Emit cupped sound
			var sound = PlaySound( CuppedSound.Name );
			sound.SetVolume( 1.0f );
			sound.SetPitch( Rand.Float(0.75f, 1.25f) );

			Particles.Create( "particles/ball_trail.vpcf", Position + Vector3.Up * 8 );

			Cupped = true;
		}

		public void ResetPosition( Vector3 position, Angles direction )
		{
			// Reset all velocity
			PhysicsBody.Velocity = Vector3.Zero;
			PhysicsBody.AngularVelocity = Vector3.Zero;
			PhysicsBody.ClearForces();
			PhysicsBody.ClearTorques();

			Position = position;
			PhysicsBody.Position = position;
			ResetInterpolation();

			Moving = false;
			Cupped = false;

			// Tell the player we reset the ball
			PlayerResetPosition( To.Single(this), position, direction );
		}

		[ClientRpc]
		protected void PlayerResetPosition( Vector3 position, Angles angles )
		{
			(Camera as FollowBallCamera).Angles = angles;
		}
	}
}
