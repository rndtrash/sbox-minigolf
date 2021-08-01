using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Minigolf
{
	partial class GolfGame : Game
	{
		public Course Course { get; set; }

		[Net] public bool WaitingToStart { get; set; } = true;
		[Net] public float StartTime { get; set; }

		public bool GameStarted { get; set; }

		public List<Client> ReadyClients { get; set; }
		public List<Client> PlayingClients { get; set; }

		public static GolfGame Instance => Current as GolfGame;

		public GolfGame()
		{
			// easy way for now.. todo look into actual clientside huds?
			if (IsServer)
            {
				_ = new GolfHUD();
			}

			if (IsClient)
			{
				Log.Info( "Game constructor Entity.All:" );
				foreach ( var ent in Entity.All )
					Log.Info( $"\t{ent}" );
			}
		}

		public override void ClientJoined( Client cl )
		{
			// Base game adds a client joined to chatbox
			base.ClientJoined( cl );

			// Sends the course names and junk to the client
			Course.SendCourseInfo( To.Single( cl ) );
		}

		[ServerCmd("minigolf_start")]
		static void StartGameS()
		{
			(Current as GolfGame).StartGame();
		}

		public void StartGame()
		{
			// TODO: Only do clients marked as ready.
			PlayingClients = new List<Client>( Client.All );

			GameStarted = true;
			WaitingToStart = false;

			// TODO: RESET?
			// Course.Reset();

			// Spawn balls for all clients
			foreach ( var cl in PlayingClients )
			{
				var ball = new GolfBall();
				ball.ResetPosition( Course.CurrentHole.SpawnPosition, Course.CurrentHole.SpawnAngles );

				// Set the client to control the ball
				cl.Pawn = ball;

				// Fixes interpolation (https://github.com/Facepunch/sbox-issues/issues/492)
				ball.Owner = null;
			}
		}

		public override void PostLevelLoaded()
		{
			Host.AssertServer();

			// Make sure we load it before setting the reference
			var course = new Course();
			course.LoadFromMap();

			Course = course;

			WaitingToStart = true;
			StartTime = (float)Math.Floor(Time.Now + 5.0f);
		}

		public override ICamera FindActiveCamera()
		{
			// TODO: Set this dynamically from the map
			if (WaitingToStart)
			{
				StaticCamera camera = new StaticCamera( new Vector3( -303.42f, 191.58f, 175.11f ), new Angles( 10.31f, -37.59f, 0 ) );
				return camera;
			}

			return base.FindActiveCamera();
		}
	}
}
