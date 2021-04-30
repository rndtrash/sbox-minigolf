using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minigolf
{
	/// <summary>
	/// Player golf ball
	/// </summary>
	[Library("minigolf_ball")]
	public partial class PlayerBall : ModelEntity
	{
		[Net] public bool IsMoving { get; set; }
		public bool InHole { get; set; }

		static readonly SoundEvent BounceSound = new("sounds/minigolf.ball_bounce1.vsnd");
		// [Net] public Particles Trail { get; set; }

		// clientside only
		public Particles PowerArrows { get; set; }

		// Clientside only
		public ModelEntity Quad { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			SetModel("models/golf_ball.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);

			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Interactive;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;

			// Disable shadows, they seem weirdly busted.
			EnableShadowCasting = true;

			Transmit = TransmitType.Always;

			// Create a networked trail for all players to see
			// Trail = Particles.Create("particles/ball_trail.vpcf");
			// Trail.SetEntity(0, this);

			// fiddlsticks
			// PhysicsBody.Mass = 30.0f;
			// PhysicsBody.AngularDamping = 0.8f;
			// PhysicsBody.LinearDamping = 0.8f;


		}

        public override void OnNewModel(Model model)
        {
            base.OnNewModel(model);

			if (Host.IsServer)
				return;

			Quad = new ModelEntity();
			Quad.SetModel("models/minigolf.ball_quad.vmdl");
			Quad.WorldPos = WorldPos; // :/
        }

		[Event( "frame" )]
		public void OnFrame()
        {
			if (Quad == null)
				return;

			var player = Player.Local as GolfPlayer;
			if (player == null) return;

			// only rotate your own ball
			if (Owner != player)
				return;

			Quad.RenderAlpha = IsMoving ? 0.0f : 1.0f;

			// keep the quad under the ball
			Quad.WorldPos = WorldPos - (Vector3.Up * 7.99f);

			var camera = player.BallCamera;
			if (camera == null) return;

			Quad.WorldRot = Rotation.FromYaw(player.BallCamera.Angles.yaw + 180);

			var power = player.ShotPower;
			var powerS = power / 100.0f; // 0-1

			var yawRadians = player.BallCamera.Angles.yaw * (MathF.PI / 180);

			if (power <= 0)
			{
				if (PowerArrows == null)
					return;

				PowerArrows.Destroy(true);
				PowerArrows = null;

				return;
			}

			if (PowerArrows == null)
				PowerArrows = Particles.Create("particles/power_arrow.vpcf");

			var moveDir = Angles.AngleVector(new Angles(0, player.BallCamera.Angles.yaw, 0)) * (0.1f + powerS);

			PowerArrows.SetPos(0, WorldPos - Vector3.Up * 7.5f);
			PowerArrows.SetPos(1, new Vector3(powerS, 0, yawRadians));
			PowerArrows.SetPos(2, moveDir);
		}

		/// <summary>
		/// Do our own physics collisions, we create a fun bouncing effect this way and handle collision sounds.
		/// </summary>
		/// <param name="eventData"></param>
		protected override void OnPhysicsCollision(CollisionEventData eventData)
		{
			// Walls are non world cause it's fucked
			if (eventData.Entity.IsWorld)
				return;

			// Don't do ridiculous bounces upwards, just bounce off walls mainly
			if (Vector3.Up.Dot(eventData.Normal) >= -0.35)
            {
				var reflect = Vector3.Reflect(eventData.PreVelocity.Normal, eventData.Normal.Normal).Normal;
				var newSpeed = Math.Max(eventData.PreVelocity.Length, eventData.Speed);

				DebugOverlay.Line(eventData.Pos, eventData.Pos - (eventData.PreVelocity.Normal * 64.0f), 5);
				DebugOverlay.Line(eventData.Pos, eventData.Pos + (reflect * 64.0f), 5);

				PhysicsBody.Velocity = reflect * newSpeed * 0.8f;
				PhysicsBody.AngularVelocity = Vector3.Zero;

				var particle = Particles.Create("particles/ball_hit.vpcf", eventData.Pos);
				particle.SetPos(0, eventData.Pos);
				particle.SetForward(0, reflect);
				particle.Destroy(false);

				// Collision sound happens at this point, not entity
				var sound = Sound.FromWorld(BounceSound.Name, eventData.Pos);
				sound.SetVolume(1.0f); // todo: scale this based on speed (it can go above 1.0)
			}
		}
	}
}
