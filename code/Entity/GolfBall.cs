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
	}
}
