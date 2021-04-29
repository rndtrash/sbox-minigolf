using Sandbox;
using System;

namespace Minigolf
{
	public class FollowBallCamera : BaseCamera
	{
		public Angles Angles;
		public float Distance = 150.0f;
		public float MinDistance => 100.0f;
		public float MaxDistance => 500.0f;
		public float DistanceStep => 50.0f;


		/// <summary>
		/// Entity to look at
		/// </summary>
		public Entity TargetEntity { get; set; }

		public override void Update()
		{
			var player = Player.Local as GolfPlayer;
			if (player == null) return;

			var ball = player.Ball;
			if (ball == null) return;

			Pos = ball.WorldPos;

			Pos += Vector3.Up * (16 + (ball.CollisionBounds.Center.z * ball.WorldScale));
			Rot = Rotation.From(Angles);

			Pos = Pos + Rot.Backward * Distance;
			FieldOfView = 50;

			Viewer = null;
		}

		public override void BuildInput(ClientInput input)
		{
			Distance = Math.Clamp(Distance + (-input.MouseWheel * Time.Delta * 100.0f * DistanceStep), MinDistance, MaxDistance);

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
