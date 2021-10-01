﻿using Sandbox;

namespace Minigolf
{
	public partial class Ball
	{
		// Everything in here is clientside only, it's easier to manipulate effects that way.

		static readonly SoundEvent BounceSound = new( "sounds/minigolf.ball_bounce1.vsnd" );

		Particles Trail { get; set; }
		Particles Arrow { get; set; }
		Particles Circle { get; set; }

		PowerArrow PowerArrow { get; set; }

		// TraceResult of a downwards trace run every clientside frame.
		TraceResult DownTrace { get; set; }

		private void CreateParticles()
		{
			// Create all particles clientside, this gives us authority over them to set control points clientside.
			Host.AssertClient();

			var ballRadius = CollisionBounds.Size.z / 2;

			Trail = Particles.Create( "particles/ball_trail.vpcf", this, "" );
			Trail.SetPosition( 1, Vector3.One ); // Color

			Circle = Particles.Create( "particles/ball_circle.vpcf", this, "" );
			Circle.SetPosition( 1, Vector3.Down * ballRadius + Vector3.Up * 0.01f );
		}

		[Event.Frame]
		private void Frame()
		{
			DownTrace = Trace.Ray( Position, Position + Vector3.Down * (CollisionBounds.Size.z) ).Ignore( this ).WorldOnly().Run();

			if ( !Moving )
			{
				var ballRadius = CollisionBounds.Size.z / 2;

				if ( Circle == null )
				{
					Circle = Particles.Create( "particles/ball_circle.vpcf", this, "" );
					Circle.SetPosition( 1, Vector3.Down * ballRadius + Vector3.Up * 0.01f );
				}

				Circle.SetPosition( 2, DownTrace.Normal );
			}
			else
			{
				Circle?.Destroy( true );
				Circle = null;
			}

			if ( Local.Pawn == this )
			{
				AdjustArrow();
			}
		}

		private void AdjustArrow()
		{
			// Only show the arrow if we're charging a shot, delete otherwise.
			if ( ShotPower.AlmostEqual( 0 ) )
			{
				if ( Arrow != null )
				{
					Arrow.Destroy(true);
					Arrow = null;
				}

				return;
			}

			if ( Game.Current.BallCamera is not FollowBallCamera camera )
				return;

			/*if ( Arrow == null )
				Arrow = Particles.Create( "particles/ball_arrow.vpcf", this, "" );

			var ArrowStart = Position + Vector3.Down * (CollisionBounds.Size.z / 2) + Vector3.Up * 0.01f;
			var ArrowEnd = ArrowStart + Angles.AngleVector( new Angles( 0, camera.Angles.yaw, 0 ) ) * ShotPower;

			Arrow.SetPosition( 0, ArrowStart );
			Arrow.SetPosition( 1, ArrowEnd );

			DebugOverlay.Line( ArrowStart, ArrowEnd );

			// Arrow.SetForward( 1, Angles.AngleVector( new Angles( 0, camera.Angles.yaw, 0 ) ) );
			// Arrow.SetPosition( 2, Vector3.One * ShotPower );*/

			if ( !PowerArrow.IsValid() )
				PowerArrow = new();

			var direction = Angles.AngleVector( new Angles( 0, camera.Angles.yaw, 0 ) );

			// TODO: hardcoded size
			PowerArrow.Position = Position + Vector3.Down * 2.99f + direction * 5.0f;
			PowerArrow.Direction = direction;
			PowerArrow.Power = ShotPower;
		}
	}
}
