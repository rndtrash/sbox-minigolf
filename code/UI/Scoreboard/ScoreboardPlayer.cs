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
	public partial class ScoreboardPlayer : Panel
	{
		public Label PlayerName { get; set; }
		public Image PlayerAvatar { get; set; }
		public Panel ScoresPanel { get; set; }
		
		Label TotalScoreLabel { get; set; }

		public Client Client;

		Dictionary<int, Label> Scores = new();

		public ScoreboardPlayer( Client client, Panel panel ) : base( panel )
		{
			Client = client;

			PlayerName.Text = Client.Name;
			PlayerAvatar.Texture = Texture.Load( $"avatar:{Client.SteamId}" );

			for ( int i = 1; i < 17; i++ )
			{
				Scores[i] = ScoresPanel.Add.Label( $"-" );
			}

		}

		public override void Tick()
		{
			var score = Client.Components.Get<ScoreComponent>();
			if ( score == null ) return;

			foreach( var pair in score.Scores )
			{
				Scores[pair.Key].Text = $"{pair.Value}";
			}

			TotalScoreLabel.Text = $"{score.Total}";

			// PlayerName.Text = Client.Name;
			// PlayerAvatar.Texture = Texture.Load( $"avatar:{Client.SteamId}" );

		}
	}
}
