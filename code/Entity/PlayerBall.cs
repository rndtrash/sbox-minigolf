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
		protected override void OnPhysicsCollision(Entity hitEntity, float speed, float timeDelta)
		{
			// todo: remove this
			if (!hitEntity.IsWorld)
				return;

			PlaySound("ballcollision_standard");

			// todo when more data exists:
			var HitNormal = new Vector3(0, 0, 0);
			var OldVelocity = new Vector3(0, 0, 0);

			if (false && Vector3.Up.Dot(HitNormal) >= -0.35)
            {
				// calculate our bounce normal
				var normal = OldVelocity.Normal;

				var dot = HitNormal.Dot(normal * -1);
				var reflect = (2 * HitNormal * dot) + normal;
				var newSpeed = Math.Max(OldVelocity.Length, speed);

				Velocity = reflect * newSpeed * 0.8f;

				// make particle effect
				// play bounce sound
			}

			base.OnPhysicsCollision(hitEntity, speed, timeDelta);
		}
	}
}
