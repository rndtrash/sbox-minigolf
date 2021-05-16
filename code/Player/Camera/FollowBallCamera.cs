using Sandbox;
using System;

namespace Minigolf
{
	public class FollowBallCamera : BaseCamera
	{
		public Angles Angles;

		private float Distance;
		private float distanceTarget;

		public float MinDistance => 50.0f;
		public float MaxDistance => 400.0f;
		public float DistanceStep => 50.0f;

		public FollowBallCamera()
		{
			Distance = 150;
			distanceTarget = Distance;
		}

		public override void Update()
		{
			var player = Player.Local as GolfPlayer;
			if ( player == null ) return;

			var ball = player.ActiveChild as GolfBall;
			if ( ball == null ) return;

			Pos = ball.WorldPos + Vector3.Up * (24 + (ball.CollisionBounds.Center.z * ball.WorldScale));
			Rot = Rotation.From( Angles );

			distanceTarget = distanceTarget.LerpTo( Distance, Time.Delta * 5.0f );

			var tr = Trace.Ray( Pos, Pos + Rot.Backward * Distance )
				.Ignore( player )
				.WorldOnly()
				.Radius( 8 )
				.Run();

			if ( tr.Hit )
			{
				distanceTarget = Math.Min( distanceTarget, tr.Distance );
			}

			Pos += Rot.Backward * distanceTarget;

			ball.RenderAlpha = Math.Clamp( (distanceTarget - 25.0f) / 50.0f, 0.0f, 1.0f );

			FieldOfView = 90;
			Viewer = null;
		}

		public override void BuildInput(ClientInput input)
		{
			Distance = Math.Clamp( Distance + (-input.MouseWheel * DistanceStep), MinDistance, MaxDistance);

			Angles.yaw += input.AnalogLook.yaw;

			if (!input.Down(InputButton.Attack1))
				Angles.pitch += input.AnalogLook.pitch;

			Angles = Angles.Normal;

			if (!input.Down(InputButton.Attack1))
				Angles.pitch = Angles.pitch.Clamp(0, 89);

			base.BuildInput(input);
		}
	}
}
