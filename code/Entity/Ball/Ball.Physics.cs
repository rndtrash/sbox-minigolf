﻿using Sandbox;
using System;

namespace Minigolf
{
	public partial class Ball
	{
		static float PowerMultiplier => 5000.0f;
		static float DefaultLinearDamping => 0.1f;
		static float DefaultAngularDamping => 4.00f;

		[Event.Tick.Server]
		protected void AdjustPhysics()
		{
			if ( !Velocity.Length.AlmostEqual( 0.0f, 0.1f ) )
				Moving = true;
			else
			{
				if ( Moving )
					Velocity = Vector3.Zero;
				Moving = false;
			}

			// If the ball is in the hole, do nothing
			if ( Cupped )
				return;

			AdjustDamping();
		}

		/// <summary>
		/// We adjust the ball's linear / angular damping based on the surface.
		/// This can be done clientside for prediction.
		/// </summary>
		protected void AdjustDamping()
		{
			var downTrace = Trace.Ray( Position, Position + Vector3.Down * CollisionBounds.Size.z );
			downTrace.HitLayer( CollisionLayer.Solid );
			downTrace.Ignore( this );
			var downTraceResult = downTrace.Run();

			if ( Debug )
			{
				DebugOverlay.Line( downTraceResult.StartPos, downTraceResult.EndPos );

				DebugOverlay.Text( Position, $"Sleeping: { PhysicsBody.IsSleeping() }" );
				DebugOverlay.Text( Position + Vector3.Up * 5f, $"Velocity: { PhysicsBody.Velocity.Length }" );

				DebugOverlay.ScreenText( 1, $"Tick: { Time.Tick }" );

				// if ( downTraceResult.Entity.IsValid() )
				// 	DebugOverlay.Text( downTraceResult.StartPos, $"e: {downTraceResult.Entity.EngineEntityName}" );
			}

			// We are in the air, do nothing? (Maybe we could adjust something to make ball airtime feel nicer?)
			if ( !downTraceResult.Hit )
				return;

			// Give ourselves a shit load of damping at a low velocity, brings the ball to a faster stop in a more natural way.
			if ( Velocity.Length < 5.0f )
			{
				PhysicsBody.LinearDamping = DefaultLinearDamping * 50.0f;
				PhysicsBody.AngularDamping = DefaultAngularDamping * 50.0f;

				return;
			}


			// See if we're on a flat surface by checking the dot product of the surface normal.
			if ( downTraceResult.Normal.Dot( Vector3.Up ).AlmostEqual( 1, 0.001f ) )
			{
				switch ( downTraceResult.Surface.Name )
				{
					case "minigolf.sand":
						PhysicsBody.LinearDamping = 2.5f;
						PhysicsBody.AngularDamping = 2.5f;
						break;
					case "minigolf.ice":
						PhysicsBody.LinearDamping = 0.25f;
						PhysicsBody.AngularDamping = 0.00f;
						break;
					default:
						PhysicsBody.LinearDamping = DefaultLinearDamping;
						PhysicsBody.AngularDamping = DefaultAngularDamping;
						break;
				}

				if ( downTraceResult.Entity is SpeedBoost speedBoost )
				{
					// TODO: Multiply by delta time
					var velocity = PhysicsBody.Velocity;
					velocity += Angles.AngleVector( speedBoost.MoveDir ) * speedBoost.SpeedMultiplier;

					PhysicsBody.Velocity = velocity;
				}

				return;
			}

			// We must be on a hill, we can detect if it's up hill or down hill by doing a forward trace
			var trace = Trace.Ray( Position, Position + PhysicsBody.Velocity.WithZ( 0 ) );
			trace.HitLayer( CollisionLayer.Debris );
			trace.Ignore( this );
			var traceResult = trace.Run();

			if ( traceResult.Hit )
			{
				PhysicsBody.LinearDamping = 0.015f;
				PhysicsBody.AngularDamping = 2.00f;
				return;
			}

			PhysicsBody.LinearDamping = 0.0f;
			PhysicsBody.AngularDamping = 1.0f;
		}

		/// <summary>
		/// Do our own physics collisions, we create a fun bouncing effect this way and handle collision sounds.
		/// </summary>
		/// <param name="eventData"></param>
		protected override void OnPhysicsCollision( CollisionEventData eventData )
		{
			// We only want to override collision with sides, those should be func_brush or similar.
			if ( eventData.Entity.IsWorld )
				return;

			// Let the engine handle the low velocity collisions
			// if ( eventData.PreVelocity.Length < 10 )
			// 	return;

			// Hard code collisions with the wall for now.
			if ( eventData.Entity is Wall wall )
			{
				if ( !wall.Reflect )
				{
					// Most likely a curve, reset z to avoid weirdness
					PhysicsBody.Velocity = PhysicsBody.Velocity.WithZ( 0 );
					return;
				}

				ReflectBall( eventData, wall.ReflectMultiplier );
				return;
			}
		}

		protected void ReflectBall( CollisionEventData eventData, float multiplier )
		{
			var reflect = Vector3.Reflect( eventData.PreVelocity.Normal, eventData.Normal.Normal ).Normal;

			var normalDot = eventData.PreVelocity.Normal.Dot( eventData.Normal );

			// Don't do any reflection if we hit it at such an angle
			if ( normalDot <= 0.10 )
				return;

			// Collision sound happens at this point, not entity
			var sound = Sound.FromWorld( BounceSound.Name, eventData.Pos );
			sound.SetVolume( 0.2f + Math.Clamp( eventData.Speed / 1250.0f, 0.0f, 0.8f ) );
			sound.SetPitch( 0.5f + Math.Clamp( eventData.Speed / 1250.0f, 0.0f, 0.5f ) );

			var particle = Particles.Create( "particles/ball_hit.vpcf", eventData.Pos );
			particle.SetPosition( 0, eventData.Pos );
			particle.Destroy( false );

			var newSpeed = Math.Max( eventData.PreVelocity.Length, eventData.Speed );
			newSpeed *= multiplier;

			// Adjust the speed depending on the hit normal, slight hit = more speed
			newSpeed *= (1 - normalDot / 2);

			var newVelocity = reflect * newSpeed;

			// TODO: not a fan of this, should determine by the dot normal
			newVelocity.z = 0;

			PhysicsBody.Velocity = newVelocity;
			PhysicsBody.AngularVelocity = Vector3.Zero;

			if ( Debug )
			{
				DebugOverlay.Text( eventData.Pos, $"V {eventData.PreVelocity.Length} -> {newSpeed}", 5f );
				DebugOverlay.Text( eventData.Pos + Vector3.Up * 8, $"N. {normalDot}", 5f );
				DebugOverlay.Line( eventData.Pos, eventData.Pos - (eventData.PreVelocity.Normal * 64.0f), 5f );
				DebugOverlay.Line( eventData.Pos, eventData.Pos + (reflect * 64.0f), 5f );
			}
		}
	}
}
