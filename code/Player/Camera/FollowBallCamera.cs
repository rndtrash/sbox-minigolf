using Sandbox;
using System;

namespace Minigolf
{
	public class FollowBallCamera : BaseCamera
	{
		public Angles Angles;

		private float actualDistance = 150.0f;
		private float distanceVel = 0.0f;

		public float Distance = 150.0f;
		public float MinDistance => 100.0f;
		public float MaxDistance => 400.0f;
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

			Pos += Vector3.Up * (24 + (ball.CollisionBounds.Center.z * ball.WorldScale));
			Rot = Rotation.From(Angles);

			// SmoothDamp our camera zoom
			actualDistance = MathFUtil.SmoothDamp(actualDistance, Distance, ref distanceVel, 0.5f, 10000.0f, Time.Delta);

			Pos = Pos + Rot.Backward * actualDistance;
			FieldOfView = 90;

			Viewer = null;
		}

		public override void BuildInput(ClientInput input)
		{
			Distance = Math.Clamp(Distance + (-input.MouseWheel * DistanceStep), MinDistance, MaxDistance);

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
