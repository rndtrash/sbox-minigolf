using Sandbox;
using System;

namespace Minigolf
{
	[Library("minigolf_ball")]
	public partial class GolfBall : ModelEntity, IPlayerControllable
	{
		[Net] public GolfPlayer Player { get; set; }

		[ServerVar( "minigolf_ball_debug" )]
		public static bool Debug { get; set; } = false;

		[Net] public bool Moving { get; set; } = false;
		[Net] public bool Cupped { get; set; } = false;

		/*public GolfBall( GolfPlayer player )
		{
			Player = player;
		}*/

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

		public void ResetPosition( Vector3 position, Angles direction )
		{
			// Reset all velocity
			PhysicsBody.Velocity = Vector3.Zero;
			PhysicsBody.AngularVelocity = Vector3.Zero;
			PhysicsBody.ClearForces();
			PhysicsBody.ClearTorques();

			WorldPos = position;
			PhysicsBody.Pos = position;
			ResetInterpolation();

			Moving = false;
			Cupped = false;

			// Tell the player we reset the ball
			PlayerResetPosition( Player, position, direction );
		}

		[ClientRpc]
		protected void PlayerResetPosition( Vector3 position, Angles angles )
		{
			var player = Sandbox.Player.Local as GolfPlayer;
			if ( player == null ) return;

			var camera = player.BallCamera;
			camera.Angles = angles;
		}
	}
}
