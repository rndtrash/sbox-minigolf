using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minigolf
{
	public partial class StartingGame : Panel
	{
		public static StartingGame Current;

		private Panel playersContainer;

		private Button readyButton;

		Dictionary<Client, Panel> players = new Dictionary<Client, Panel>();

		public StartingGame()
		{
			Current = this;

			StyleSheet.Load("/ui/StartingGame.scss");

			Add.Image( "ui/logo_golf.png", "logo" );

			playersContainer = Add.Panel( "players" );

			var btns = Add.Panel( "buttons" );
			readyButton = btns.Add.Button( "Start Game", "ready", () => { ConsoleSystem.Run( "minigolf_start" ); } );
			btns.Add.Button( "Settings", "settings" );

			Add.Label( "This is a tip to help you play the game.", "tip" );
		}

		protected void AddClient( Client client )
		{
			Log.Info( $"{client.Name} - avatar:{client.SteamId}" );

			var p = playersContainer.Add.Panel();
			p.Add.Image( $"avatar:{client.SteamId}" );
			p.Add.Label( $"{client.Name}" );

			players[client] = p;
		}

		public override void Tick()
		{
			base.Tick();

			var game = Sandbox.Game.Current as GolfGame;
			if ( !game.WaitingToStart )
				AddClass( "hide" );

			// bigLabel.Text = "Game starting...";
			// messageLabel.Text = $"< {TimeSpan.FromSeconds( Math.Clamp( Math.Ceiling( game.StartTime - Sandbox.Time.Now ), 0, 300 ) ).ToString( @"mm\:ss" )} > ";

			foreach ( var client in Client.All )
			{
				// TODO: Delete no longer existed clients
				if ( players.ContainsKey( client ) )
					continue;

				AddClient( client );
			}
		}

		public void Hide()
        {
			// (GolfHUD.Current as GolfHUD).Fade = false;
			RemoveClass("hide");
		}
    }

}
