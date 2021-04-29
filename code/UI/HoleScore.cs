
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
			holeContainer.Add.Label("HOLE");
			holeLabel = holeContainer.Add.Label("0");

			var parContainer = Add.Panel("par");
			parContainer.Add.Label("PAR");
			parLabel = parContainer.Add.Label("0");

			var strokeContainer = Add.Panel("stroke");
			strokeContainer.Add.Label("STROKE");
			strokeLabel = strokeContainer.Add.Label("0");
		}

		public override void Tick()
		{
			var game = Game.Current as GolfGame;
			if (game == null) return;

			var player = Player.Local as GolfPlayer;
			if (player == null) return;

			holeLabel.Text = $"{game.CurrentHole}";
			parLabel.Text = $"{game.HolePar}";
			strokeLabel.Text = $"{player.Strokes}";
		}
	}

}