﻿using Sandbox;
using System;

namespace Minigolf
{
	public partial class GolfBall
	{
		[ServerVar( "minigolf_ball_reflect_multiplier" )]
		public static float ReflectMultiplier { get; set; } = 0.75f;
		[ServerVar( "minigolf_ball_linear_damping" )]
		public static float DefaultLinearDamping { get; set; } = 0.05f;
		[ServerVar( "minigolf_ball_angular_damping" )]
		public static float DefaultAngularDamping { get; set; } = 4.00f;

		public void OnPlayerControlTick( Player owner )
		{
			// If the ball is in the hole, do nothing
			if ( Cupped )
				return;

			// TODO: Check if the ball is determined ready to hit again
			// TODO: Do out of bounds check here instead

			// Do physics to change dampening
			var trace = Trace.Ray( WorldPos, WorldPos + Vector3.Down * 8 );
			trace.HitLayer( CollisionLayer.Debris );
			trace.Ignore( this );
			var traceResult = trace.Run();

			if ( !traceResult.Hit )
				return;

			var normalDot = traceResult.Normal.Dot( Vector3.Up );
			// DebugOverlay.Text(WorldPos + Vector3.Up * 8.0f, $"N.Dot: {normalDot}");

			// Flat surface
			if ( normalDot.AlmostEqual( 1 ) )
			{
				// PhysicsBody.LinearDamping = 0.05f;
				// PhysicsBody.AngularDamping = 4.00f;

				PhysicsBody.LinearDamping = DefaultLinearDamping;
				PhysicsBody.AngularDamping = DefaultAngularDamping;

				if ( traceResult.Surface.Name == "minigolf.ice" )
				{
					PhysicsBody.LinearDamping = 0.25f;
					PhysicsBody.AngularDamping = 0.00f;
				}

				if ( traceResult.Surface.Name == "minigolf.sand" )
				{
					PhysicsBody.LinearDamping = 2.5f;
					PhysicsBody.AngularDamping = 2.5f;
				}

				return;
			}
		}

		/// <summary>
		/// We adjust the ball's linear / angular damping based on the surface.
		/// This can be done clientside for prediction.
		/// </summary>
		protected void AdjustDamping()
		{
			var downTrace = Trace.Ray( WorldPos, WorldPos + Vector3.Down * OOBBox.Size.z );
			downTrace.HitLayer( CollisionLayer.Solid );
			downTrace.Ignore( this );
			var downTraceResult = downTrace.Run();

			if ( Debug )
				DebugOverlay.Line( downTraceResult.StartPos, downTraceResult.EndPos );

			// We are in the air, do nothing? (Maybe we could adjust something to make ball airtime feel nicer?)
			if ( !downTraceResult.Hit )
				return;

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

				return;
			}

			// We must be on a hill, we can detect if it's up hill or down hill by doing a forward trace
			var trace = Trace.Ray( WorldPos, WorldPos + PhysicsBody.Velocity.WithZ( 0 ) );
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

		Vector3 prevWorldPos;
		Vector3 clientVelocity;

		/// <summary>
		/// Velocity is currently null on the client, let's figure it out here.
		/// </summary>
		protected void ClientVelocityWorkaround()
		{
			clientVelocity = WorldPos - prevWorldPos;

			if ( Trail == null )
				return;

			Trail.SetPos( 0, WorldPos );
			Trail.SetPos( 1, prevWorldPos );

			var clientVelocityLength = clientVelocity.Length;
			Trail.SetPos( 2, new Vector3( clientVelocityLength ) );

			prevWorldPos = WorldPos;
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

			if ( eventData.Speed < 10 )
				return;

			// Ball randomly bounces off the ground, this should stop it.
			// if (Vector3.Up.Dot(eventData.Normal) < -0.35)
			// 	return;

			var reflect = Vector3.Reflect( eventData.PreVelocity.Normal, eventData.Normal.Normal ).Normal;

			if ( Debug )
			{
				DebugOverlay.Text( eventData.Pos, $"{eventData.Speed}", 5f );
				DebugOverlay.Line( eventData.Pos, eventData.Pos - (eventData.PreVelocity.Normal * 64.0f), 5f );
				DebugOverlay.Line( eventData.Pos, eventData.Pos + (reflect * 64.0f), 5f );
			}

			// TODO: DETECT CURVES

			// Collision sound happens at this point, not entity
			var sound = Sound.FromWorld( BounceSound.Name, eventData.Pos );
			sound.SetVolume( 0.8f );
			sound.SetPitch( 0.5f + Math.Clamp( eventData.Speed / 1250.0f, 0.0f, 0.5f ) );

			var particle = Particles.Create( "particles/ball_hit.vpcf", eventData.Pos );
			particle.SetPos( 0, eventData.Pos );
			particle.Destroy( false );

			var newSpeed = Math.Max( eventData.PreVelocity.Length, eventData.Speed );

			var newVelocity = reflect * newSpeed * ReflectMultiplier;
			// newVelocity.z = 0;

			PhysicsBody.Velocity = newVelocity;
			PhysicsBody.AngularVelocity = Vector3.Zero;
		}
	}
}