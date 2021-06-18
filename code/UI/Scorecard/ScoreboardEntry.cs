
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace Minigolf
{
	public partial class ScoreboardEntry : Panel
	{
		public PlayerScore.Entry Entry;

		public Label PlayerName;

		public ScoreboardEntry()
		{
			PlayerName = Add.Label( "PlayerName", "name" );
			var panel = Add.Panel();
			for (int i = 1; i < 9; i++)
				panel.Add.Label("0");
		}

		public virtual void UpdateFrom( PlayerScore.Entry entry )
		{
			Entry = entry;

			PlayerName.Text = entry.GetString( "name" );

			SetClass( "me", Local.Client != null && entry.Get<ulong>( "steamid", 0 ) == Local.Client.SteamId );
		}
	}
}
