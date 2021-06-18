using Sandbox;
using System;

namespace Minigolf
{
	public class FollowBallCamera : ICamera
	{
		public Angles Angles;
		public Vector3 Position;

		private float Distance;
		private float distanceTarget;

		public float MinDistance => 50.0f;
		public float MaxDistance => 400.0f;
		public float DistanceStep => 50.0f;

		public GolfBall Ball;

		public FollowBallCamera( GolfBall ball )
		{
			Ball = ball;

			Distance = 150;
			distanceTarget = Distance;
		}

		public override void Build( ref CameraSetup camSetup )
		{
			if ( !Ball.IsValid() ) return;

			var pos = Ball.Position + Vector3.Up * (24 + (Ball.CollisionBounds.Center.z * Ball.Scale));
			var rot = Rotation.From( Angles );

			distanceTarget = distanceTarget.LerpTo( Distance, Time.Delta * 5.0f );

			var tr = Trace.Ray( pos, pos + rot.Backward * Distance )
				.Ignore( Ball )
				.WorldOnly()
				.Radius( 8 )
				.Run();

			if ( tr.Hit )
			{
				distanceTarget = Math.Min( distanceTarget, tr.Distance );
			}

			pos += rot.Backward * distanceTarget;

			// ball.RenderAlpha = Math.Clamp( (distanceTarget - 25.0f) / 50.0f, 0.0f, 1.0f );

			camSetup.Position = pos;
			camSetup.Rotation = rot;
			camSetup.FieldOfView = 90;
		}

		public override void BuildInput( InputBuilder input )
		{
			Distance = Math.Clamp( Distance + (-input.MouseWheel * DistanceStep), MinDistance, MaxDistance );

			Angles.yaw += input.AnalogLook.yaw;

			if ( !input.Down( InputButton.Attack1 ) )
				Angles.pitch += input.AnalogLook.pitch;

			Angles = Angles.Normal;

			if ( !input.Down( InputButton.Attack1 ) )
				Angles.pitch = Angles.pitch.Clamp( 0, 89 );
		}
	}
}
