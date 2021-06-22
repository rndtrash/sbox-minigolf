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

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );

			if ( cl != Local.Client ) return;

			// RenderColor = ColorConvert.HSLToRGB( 100 + (int)(Math.Sin(Time.Now) * 100), 1.0f, 0.5f ).ToColor32();

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

			if ( Camera is not FollowBallCamera camera )
				return;

			if ( !PowerArrow.IsValid() )
				PowerArrow = new();

			var direction = Angles.AngleVector( new Angles( 0, camera.Angles.yaw, 0 ) );

			// TODO: hardcoded size
			PowerArrow.Position = Position + Vector3.Down * 2.99f + direction * 5.0f;
			PowerArrow.Direction = direction;
			PowerArrow.Power = ShotPower / 100.0f;
		}
	}
}
