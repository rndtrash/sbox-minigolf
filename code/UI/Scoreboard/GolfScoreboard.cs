using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minigolf
{
	[UseTemplate]
	public partial class GolfScoreboard : Panel
	{
		// Dictionary<int, ScoreboardEntry> Entries = new();
		Panel PlayersPanel { get; set; }
		Panel SpectatorsPanel { get; set; }
		Panel HoleHeadersPanel { get; set; }
		Panel ParHeadersPanel { get; set; }

		Dictionary<Client, ScoreboardPlayer> Players = new();
		Dictionary<Client, Label> Spectators = new();

		public GolfScoreboard()
		{
			for ( int i = 0; i < 16; i++ )
			{
				HoleHeadersPanel.Add.Label( $"{ i + 1 }" );
			}

			for ( int i = 0; i < 16; i++ )
			{
				ParHeadersPanel.Add.Label( $"3" );
			}
		}

		protected override void PostTemplateApplied()
		{
			base.PostTemplateApplied();
		}

		public override void Tick()
		{
			base.Tick();

			SetClass("open", Input.Down(InputButton.Score));
			// SetClass("open", true);

			

			// TODO: Fuck all of this out of Tick, was just quicker to make like this

			foreach ( var pnl in PlayersPanel.ChildrenOfType<ScoreboardPlayer>().Where( pnl => !pnl.Client.IsValid() ) ) // or they're not a player
				pnl.Delete();

			var clients = Client.All.ToList();
			foreach ( var pnl in PlayersPanel.ChildrenOfType<ScoreboardPlayer>() )
				clients.Remove( pnl.Client );
			foreach ( var pnl in SpectatorsPanel.ChildrenOfType<Label>() )
			{
				var aaa = clients.Where( cl => cl.Name == pnl.Text ).FirstOrDefault();

				// if the player is now a player and not a spectator
				if ( Game.Current.PlayingClients.Contains( aaa ) )
				{
					pnl.Delete();
					continue;
				}

				if ( aaa != null ) clients.Remove( aaa );
			}

			foreach ( var cl in clients )
			{
				bool playing = Game.Current.PlayingClients.Contains( cl );

				if ( playing )
				{
					var pnl = new ScoreboardPlayer( cl, PlayersPanel );
					PlayersPanel.AddChild( pnl );					
				}
				else
					SpectatorsPanel.Add.Label( cl.Name );
			}


		}
	}
}
