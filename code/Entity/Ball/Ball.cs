﻿using Sandbox;

namespace Minigolf
{
	public partial class Ball : ModelEntity
	{
		[ServerVar( "minigolf_ball_debug" )]
		public static bool Debug { get; set; } = false;
		[Net] public bool InPlay { get; set; } = false;
		[Net] public bool Cupped { get; set; } = false;

		static readonly SoundEvent CuppedSound = new SoundEvent( "sounds/minigolf.ball_inhole.vsnd" );

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

			Predictable = false;
		}

		public override void ClientSpawn()
		{
			base.ClientSpawn();

			CreateParticles();
		}

		public void Cup( bool holeInOne = false )
		{
			if ( Cupped ) return;

			Cupped = true;

			var sound = PlaySound( CuppedSound.Name );
			sound.SetVolume( 1.0f );
			sound.SetPitch( Rand.Float(0.75f, 1.25f) );
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

			InPlay = false;
			Cupped = false;

			// Tell the player we reset the ball
			PlayerResetPosition( To.Single(this), position, direction );
		}

		[ClientRpc]
		protected void PlayerResetPosition( Vector3 position, Angles angles )
		{
			// (Camera as FollowBallCamera).Angles = angles;
		}
	}
}
