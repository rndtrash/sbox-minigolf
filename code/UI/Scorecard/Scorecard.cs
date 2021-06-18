using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace Minigolf
{
	public partial class Scorecard : Panel
	{
		public Panel Canvas { get; protected set; }
		public TextEntry Input { get; protected set; }

		Dictionary<int, ScoreboardEntry> Entries = new();

		public Panel Header { get; protected set; }

		public Scorecard()
		{
			StyleSheet.Load("/UI/Scorecard/Scorecard.scss");

			var container = Add.Panel("container");

			AddHeader(container);

			Canvas = container.Add.Panel("canvas");

			PlayerScore.OnPlayerAdded += AddPlayer;
			PlayerScore.OnPlayerUpdated += UpdatePlayer;
			PlayerScore.OnPlayerRemoved += RemovePlayer;

			foreach (var player in PlayerScore.All)
			{
				AddPlayer(player);
			}
		}
		public override void Tick()
		{
			base.Tick();

			// SetClass("open", Player.Local?.Input.Down(InputButton.Score) ?? false);
		}


		protected virtual void AddHeader(Panel container)
		{
			container.Add.Label("SCORECARD", "header");

			{
				var row = container.Add.Panel("row");
				row.Add.Label("HOLE");
				var holes = row.Add.Panel();
				for (int i = 1; i < 9; i++)
					holes.Add.Label(i.ToString());
			}

			{
				var row = container.Add.Panel("row");
				row.Add.Label("PAR");
				var holes = row.Add.Panel();
				for (int i = 1; i < 9; i++)
					holes.Add.Label("3");
			}
		}

		protected virtual void AddPlayer(PlayerScore.Entry entry)
		{
			var p = Canvas.AddChild<ScoreboardEntry>();
			p.UpdateFrom(entry);

			Entries[entry.Id] = p;
		}

		protected virtual void UpdatePlayer(PlayerScore.Entry entry)
		{
			if (Entries.TryGetValue(entry.Id, out var panel))
			{
				panel.UpdateFrom(entry);
			}
		}

		protected virtual void RemovePlayer(PlayerScore.Entry entry)
		{
			if (Entries.TryGetValue(entry.Id, out var panel))
			{
				panel.Delete();
				Entries.Remove(entry.Id);
			}
		}
	}
}
