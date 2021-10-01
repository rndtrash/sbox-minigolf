﻿
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace Minigolf
{
	public partial class HoleScore : Panel
	{
		public static HoleScore Current;

		private Label strokeLabel;
		private Label parLabel;
		private Label holeLabel;

		public HoleScore()
		{
			Current = this;

			StyleSheet.Load("/ui/HoleScore.scss");

			var strokeContainer = Add.Panel("stroke");
			strokeLabel = strokeContainer.Add.Label("0");

			var parContainer = Add.Panel("par");
			parContainer.Add.Label("PAR", "first" );
			parLabel = parContainer.Add.Label("0", "last");

			var holeContainer = Add.Panel("hole");
			holeContainer.Add.Label("HOLE", "first");
			holeLabel = holeContainer.Add.Label("0", "last");
		}

		public override void Tick()
		{
			if ( Game.Current == null) return;

			var hole = Game.Current.Course.CurrentHole;
			// if (hole == null) return;

			holeLabel.Text = $"{hole.Number}";
			parLabel.Text = $"{hole.Par}";
			// strokeLabel.Text = $"{player.Strokes}";

			var score = Local.Client.Components.Get<ScoreComponent>();
			if ( score == null ) return;

			strokeLabel.Text = $"{score.Score}";
		}
	}

}
