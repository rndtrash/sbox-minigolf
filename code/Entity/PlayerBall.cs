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

		// Clientside only
		public BallQuad Quad { get; set; }
		public Particles Trail { get; set; }
		public Particles PowerArrows { get; set; }


		public override void Spawn()
		{
			base.Spawn();

			SetModel("models/golf_ball.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);

			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Debris;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;

			Transmit = TransmitType.Always;
		}

        public override void OnNewModel(Model model)
        {
            base.OnNewModel(model);

			if (Host.IsServer)
				return;

			if ( !Quad.IsValid() )
			{
				Quad = new();
			}

			Trail = Particles.Create("particles/ball_trail.vpcf");
		}

		[Event("server.tick")]
		public void FixBall()
        {
			// DebugOverlay.Text( WorldPos + Vector3.Up * 8f, $"Server Velocity: {Velocity.Length}" );

			// Delete ball if the owner has disconnected
			if (!Owner.IsValid())
            {
				Delete();
				return;
            }

			// If the ball is in the hole, do nothing
			if (InHole)
				return;

			// DebugOverlay.Text(WorldPos + Vector3.Up * 4.0f, $"LinearDamping: {PhysicsBody.LinearDamping}");
			// DebugOverlay.Text(WorldPos, $"AngularDamping: {PhysicsBody.AngularDamping}");

			// TODO: Check if the ball is determined ready to hit again
			// TODO: Do out of bounds check here instead

			// Do physics to change dampening
			var trace = Trace.Ray(WorldPos, WorldPos + Vector3.Down * 8);
			trace.HitLayer(CollisionLayer.Debris);
			trace.Ignore(this);
			var traceResult = trace.Run();

			if (!traceResult.Hit)
				return;

			var normalDot = traceResult.Normal.Dot(Vector3.Up);
			// DebugOverlay.Text(WorldPos + Vector3.Up * 8.0f, $"N.Dot: {normalDot}");

			// Flat surface
			if (normalDot.AlmostEqual(1))
            {
				PhysicsBody.LinearDamping = 0.05f;
				PhysicsBody.AngularDamping = 4.00f;
				return;
			}

			var velocity = PhysicsBody.Velocity;
			velocity.z = 0;
			trace = Trace.Ray(WorldPos, WorldPos + velocity);
			trace.HitLayer(CollisionLayer.Debris);
			trace.Ignore(this);
			traceResult = trace.Run();

			if (traceResult.Hit)
            {
				PhysicsBody.LinearDamping = 0.015f;
				PhysicsBody.AngularDamping = 2.00f;
				return;
			}

			PhysicsBody.LinearDamping = 0.0f;
			PhysicsBody.AngularDamping = 1.0f;
		}

		Vector3 prevWorldPos;
		Vector3 clientVelocity;

		[Event( "client.tick")]
		protected void HackyClientVelocity()
		{
			// Calculate our velocity manually on the client.

			clientVelocity = WorldPos - prevWorldPos;

			Trail.SetPos( 0, WorldPos );
			Trail.SetPos( 1, prevWorldPos );

			var clientVelocityLength = clientVelocity.Length;
			Trail.SetPos( 2, new Vector3(clientVelocityLength) );

			// DebugOverlay.Text( WorldPos, $"Client Velocity: {clientVelocityLength * 100}" );

			prevWorldPos = WorldPos;
		}

		[Event( "frame" )]
		public void OnFrame()
        {
			if (Quad == null)
				return;

			var player = Player.Local as GolfPlayer;
			if (player == null) return;

			// only do stuff with your own ball
			if (Owner != player)
            {
				Quad.ShouldDraw = false;
				return;
			}

			// keep the quad under the ball
			Quad.WorldPos = WorldPos + (Vector3.Down * 3.99f);
			Quad.ShouldDraw = !IsMoving;

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

			PowerArrows.SetPos(0, WorldPos - Vector3.Up * 3.5f);
			PowerArrows.SetPos(1, new Vector3(powerS, 0, yawRadians));
			PowerArrows.SetPos(2, moveDir);
		}

		/// <summary>
		/// Do our own physics collisions, we create a fun bouncing effect this way and handle collision sounds.
		/// </summary>
		/// <param name="eventData"></param>
		protected override void OnPhysicsCollision(CollisionEventData eventData)
		{
			// World collision is pretty buggy
			if (eventData.Entity.IsWorld)
				return;

			if (eventData.Speed < 50)
				return;

			// var reflecta = Vector3.Reflect(eventData.PreVelocity.Normal, eventData.Normal.Normal).Normal;
			// DebugOverlay.Line(eventData.Pos, eventData.Pos - (eventData.PreVelocity.Normal * 64.0f), 5);
			// DebugOverlay.Line(eventData.Pos, eventData.Pos + (reflecta * 64.0f), 5);

			// Ball randomly bounces off the ground, this should stop it.
			// if (Vector3.Up.Dot(eventData.Normal) < -0.35)
			// 	return;

			// Time since last collision, don't make too many noises
			if (eventData.TimeDelta > 0.2)
            {
				// Collision sound happens at this point, not entity
				var sound = Sound.FromWorld(BounceSound.Name, eventData.Pos);
				sound.SetVolume(0.8f);
				sound.SetPitch(0.5f + Math.Clamp(eventData.Speed / 1250.0f, 0.0f, 0.5f));

				var particle = Particles.Create("particles/ball_hit.vpcf", eventData.Pos);
				particle.SetPos(0, eventData.Pos);
				// particle.SetForward(0, reflect);
				// todo: pass scalar to particle
				particle.Destroy(false);
			}

			// Walls are non world cause it's fucked
			// if (eventData.Entity.IsWorld)
			// 	return;

			// DebugOverlay.Text(eventData.Pos, $"{eventData.Speed}", 5f);

			var reflect = Vector3.Reflect(eventData.PreVelocity.Normal, eventData.Normal.Normal).Normal;
			var newSpeed = Math.Max(eventData.PreVelocity.Length, eventData.Speed);

			var newVelocity = reflect * newSpeed * 0.8f;
			newVelocity.z = 0;

			PhysicsBody.Velocity = newVelocity;
			PhysicsBody.AngularVelocity = Vector3.Zero;
		}
	}
}
