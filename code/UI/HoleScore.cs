
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace Minigolf
{
	public partial class HoleScore : Panel
	{
		public static HoleScore Current;

		private Label holeLabel;
		private Label parLabel;
		private Label strokeLabel;

		public HoleScore()
		{
			Current = this;

			StyleSheet.Load("/ui/HoleScore.scss");

			var holeContainer = Add.Panel("hole");
			holeContainer.Add.Label("HOLE", "first");
			holeLabel = holeContainer.Add.Label("0", "last");

			var parContainer = Add.Panel("par");
			parContainer.Add.Label("PAR", "first" );
			parLabel = parContainer.Add.Label("0", "last");

			var strokeContainer = Add.Panel("stroke");
			strokeContainer.Add.Label("STROKE", "first");
			strokeLabel = strokeContainer.Add.Label("0", "last");
		}

		public override void Tick()
		{
			var game = Game.Current as GolfGame;
			if (game == null) return;

			var hole = game.Course.CurrentHole;
			// if (hole == null) return;

			holeLabel.Text = $"{hole.Number}";
			parLabel.Text = $"{hole.Par}";
			// strokeLabel.Text = $"{player.Strokes}";
		}
	}

}
