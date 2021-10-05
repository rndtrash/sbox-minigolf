using Sandbox;
using System.Collections.Generic;

namespace Minigolf
{
	public enum GameState
	{
		WaitingForPlayers,
		Playing,
		EndOfGame
	}

	partial class Game
	{
		[Net, Change]
		public GameState State { get; set; } = GameState.WaitingForPlayers;

		[Net] public List<Client> PlayingClients { get; set; }

		public void OnStateChanged( GameState oldState, GameState newState )
		{
			Event.Run( "minigolf.state.changed", newState );
			// Pass to HUD?

			if ( newState == GameState.Playing )
			{
				NameTags = new();
			}
			else if ( NameTags.IsValid() )
			{
				NameTags.Delete();
			}
		}

		[ServerCmd]
		public static void Ready()
		{
			var client = ConsoleSystem.Caller;
			if ( client == null ) return;

			var readyComponent = client.Components.GetOrCreate<ReadyComponent>();
			readyComponent.Ready = !readyComponent.Ready;

			// TODO: move this to a tick or some shit
			foreach ( var cl in Client.All )
			{
				var rdy = cl.Components.GetOrCreate<ReadyComponent>();
				if ( !rdy.Ready ) return;
			}

			Current.StartGame();
		}

		public void StartGame()
		{
			PlayingClients = new List<Client>();
			foreach( var client in Client.All )
			{
				var readyComponent = client.Components.GetOrCreate<ReadyComponent>();
				if ( readyComponent.Ready )
					PlayingClients.Add( client );
			}

			// Spawn balls for all clients
			foreach ( var cl in PlayingClients )
			{
				cl.Components.Create<ScoreComponent>();

				var ball = new Ball();
				ball.ResetPosition( Course.CurrentHole.SpawnPosition, Course.CurrentHole.SpawnAngles );
				cl.Pawn = ball;
			}

			State = GameState.Playing;
		}

		[AdminCmd( "minigolf_force_start" )]
		public static void ForceStart()
		{
			// Force everyone to ready
			foreach ( var client in Client.All )
			{
				client.Components.GetOrCreate<ReadyComponent>().Ready = true;
			}

			Current.StartGame();
		}
	}
}
