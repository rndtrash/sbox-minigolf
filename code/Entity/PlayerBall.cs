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
		// public GolfPlayer Owner;

		public override void Spawn()
		{
			base.Spawn();

			SetModel("models/golf_ball.vmdl");

			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Interactive;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			// fiddlsticks
			// PhysicsBody.Mass = 30.0f;
			// PhysicsBody.AngularDamping = 0.8f;
			// PhysicsBody.LinearDamping = 0.8f;
		}

		/// <summary>
		/// Do our own bounce physics and collision sounds here
		/// </summary>
		/// <param name="hitEntity"></param>
		/// <param name="speed"></param>
		/// <param name="timeDelta"></param>
		protected override void OnPhysicsCollision(CollisionEventData eventData)
		{
			// We only want to adjust how we bounce off walls for now
			// if (!eventData.Entity.IsWorld)
			// 	return;

			Sandbox.UI.ChatBox.AddInformation(Player.All, $"Bounce at normal: {eventData.Normal}");

			if (true || Vector3.Up.Dot(eventData.Normal) >= -0.35)
            {
				// calculate our bounce normal
				var normal = eventData.PreVelocity.Normal;

				//var dot = eventData.Normal.Dot(normal * -1);
				//var reflect = (2 * eventData.Normal * dot) + normal;
				var reflect = Vector3.Reflect(eventData.PreVelocity.Normal, eventData.Normal.Normal).Normal;
				var newSpeed = Math.Max(eventData.PreVelocity.Length, eventData.Speed);

				Sandbox.UI.ChatBox.AddInformation(Player.All, $"Reflect normal: {reflect}");
				DebugOverlay.Line(eventData.Pos, eventData.Pos + (reflect * 32.0f), 5);

				Velocity = reflect * newSpeed * 0.8f;

				PlaySound("ballcollision_standard");

				// make particle effect
			}

			base.OnPhysicsCollision(eventData);
		}
	}
}
