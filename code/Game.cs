using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Minigolf
{
	partial class GolfGame : Game
	{
		[Net] public bool WaitingToStart { get; set; } = true;
		[Net] public float StartTime { get; set; }

		[Net]
		public Course Course { get; set; }

		public bool GameStarted { get; set; }

		public List<Client> ReadyClients { get; set; }
		public List<Client> PlayingClients { get; set; }

		public GolfGame()
		{
			// easy way for now.. todo look into actual clientside huds?
			if (IsServer)
            {
				_ = new GolfHUD();
				Course = new Course();
			}
		}

		[ServerCmd("minigolf_debug_print_sv")]
		static void PrintCourse()
		{
			var game = Current as GolfGame;
			Log.Info( $"Coruse: {game.Course}, {game.Course.CurrentHole}, {game.Course.Holes}" );
			Log.Info( $"{game.Course.Holes.Count} holes" );
			foreach(var hole in game.Course.Holes)
			{
				Log.Info( $"\t[{hole.Number}] par = {hole.Par}" );
			}
		}

		[ClientCmd( "minigolf_debug_print_cl" )]
		static void PrintCourseCl()
		{
			PrintCourse();
		}

		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );
		}

		[ServerCmd("minigolf_start")]
		static void StartGameS()
		{
			(Current as GolfGame).StartGame();
		}

		public void StartGame()
		{
			Log.Info( "Starting game?" );

			// TODO: Only do clients marked as ready.
			PlayingClients = new List<Client>( Client.All );

			GameStarted = true;
			WaitingToStart = false;

			// Set the active course
			// Spawn balls for all clients
			foreach ( var cl in PlayingClients )
			{
				Log.Info( $"Creating ball for {cl.Name}" );

				var ball = new GolfBall();
				cl.Pawn = ball;
				ball.ResetPosition( Course.CurrentHole.SpawnPosition, Course.CurrentHole.SpawnAngles );
			}
		}

		public override void PostLevelLoaded()
		{
			WaitingToStart = true;
			StartTime = (float)Math.Floor(Time.Now + 5.0f);

			if ( IsServer )
				Course.LoadFromMap();
		}

		public override ICamera FindActiveCamera()
		{
			if (WaitingToStart)
			{
				StaticCamera camera = new StaticCamera( new Vector3( -303.42f, 191.58f, 175.11f ), new Angles( 10.31f, -37.59f, 0 ) );
				return camera;
			}

			return base.FindActiveCamera();
		}
	}
}
