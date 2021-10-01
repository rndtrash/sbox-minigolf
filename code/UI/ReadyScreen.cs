using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minigolf
{
	public partial class ReadyScreenPlayer : Panel
	{		
		public Client Client { get; set; }
		// public Label NameLabel;
		public Image Avatar;

		public ReadyScreenPlayer( Client cl )
		{
			Client = cl;
			Avatar = Add.Image( $"avatarbig:{cl.SteamId}" );
			// NameLabel = Add.Label( $"{cl.Name}" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( !Client.IsValid() )
				return;

			var readyComponent = Client.GetEntity().Components.Get<ReadyComponent>();

			if ( readyComponent == null )
				return;

			SetClass( "ready", readyComponent.Ready );
		}
	}

	[UseTemplate]
	public partial class ReadyScreen : Panel
	{
		Button ReadyButton { get; set; }
		Panel PlayersContainer { get; set; }
		Label ReadyCount { get; set; }

		public void Ready()
		{
			Game.Ready();

			// var pnl = PlayersContainer.Children.First();
			// pnl.SetClass( "ready", !pnl.HasClass( "ready" ) );
		}

		public override void Tick()
		{
			base.Tick();

			// Remove any invalid clients (disconnected)
			foreach ( var playerPanel in PlayersContainer.Children.OfType<ReadyScreenPlayer>() )
			{
				if ( playerPanel.Client.IsValid() )
					continue;

				playerPanel.Delete();
			}

			// Add any new clients that aren't already in the list
			foreach ( var client in Client.All )
			{
				if ( PlayersContainer.Children.OfType<ReadyScreenPlayer>().Any( panel => panel.Client == client ) )
					continue;

				var panel = new ReadyScreenPlayer( client );
				panel.Parent = PlayersContainer;
			}

			var readyPlayers = 0;
			foreach ( var client in Client.All )
			{
				var component = client.GetEntity().Components.Get<ReadyComponent>();
				if ( component == null )
					continue;

				if ( component.Ready )
					readyPlayers++;
			}
			ReadyCount.Text = $"{readyPlayers}/{Client.All.Count} Players Ready";
		}
	}
}
