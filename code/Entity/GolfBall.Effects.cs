using Sandbox;
using System;

namespace Minigolf
{
	public partial class GolfBall
	{
		static readonly SoundEvent BounceSound = new( "sounds/minigolf.ball_bounce1.vsnd" );

		public BallQuad Quad { get; set; }
		public PowerArrow PowerArrow { get; set; }
		public Particles Trail { get; set; }

		public override void OnNewModel( Model model )
		{
			base.OnNewModel( model );

			if ( Host.IsServer )
				return;

			/*if ( !Quad.IsValid() )
			{
				Quad = new();
			}*/

			if ( Trail == null )
				Trail = Particles.Create( "particles/ball_trail.vpcf" );
		}

		[Event( "frame" )]
		public void OnFrame()
		{
			// if (Quad == null)
			// 	return;

			var localPlayer = Sandbox.Player.Local as GolfPlayer;
			if ( localPlayer == null ) return;

			if ( Player != localPlayer ) return;

			var camera = localPlayer.BallCamera;
			if ( camera == null ) return;

			/*
			// only do stuff with your own ball
			if (Player != player)
            {
				Quad.ShouldDraw = false;
				return;
			}

			// keep the quad under the ball
			Quad.WorldPos = WorldPos + (Vector3.Down * 3.99f);
			Quad.ShouldDraw = !IsMoving;*/

			// Quad.WorldRot = Rotation.FromYaw(player.BallCamera.Angles.yaw + 180);

			var power = localPlayer.ShotPower;
			var powerS = power / 100.0f; // 0-1

			if ( !PowerArrow.IsValid() )
				PowerArrow = new();

			var direction = Angles.AngleVector( new Angles( 0, localPlayer.BallCamera.Angles.yaw, 0 ) );

			// TODO: hardcoded size
			PowerArrow.WorldPos = WorldPos + Vector3.Down * 2.99f + direction * 5.0f;
			PowerArrow.Direction = direction;
			PowerArrow.Power = powerS;
		}
	}
}
