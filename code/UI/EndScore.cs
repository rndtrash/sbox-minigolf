
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minigolf
{
	public partial class EndScore : Panel
	{
		public static readonly string HoleInOne = "Hole-in-One!";
		public static readonly Dictionary<int, string> ScoreText = new Dictionary<int, string>
		{
			{ 4, "Condor" },
			{ 3, "Double Eagle" },
			{ 2, "Eagle" },
			{ 1, "Birdie" },
			{ 0, "Par" },
			{ -1, "Bogey" },
			{ -2, "Double Bogey" },
			{ -3, "Triple Bogey" },
			{ -4, "Quadruple Bogey" },
		};

		public static EndScore Current;

		private Label scoreLabel;
		private Label holeLabel;

		public EndScore()
		{
			Current = this;

			StyleSheet.Load("/ui/EndScore.scss");

			scoreLabel = Add.Label("", "score");
			holeLabel = Add.Label("", "hole");
		}

		public async Task ShowScore(int hole, int par, int strokes)
        {
			if (strokes == 1)
				scoreLabel.Text = HoleInOne;
			else
				scoreLabel.Text = ScoreText.GetValueOrDefault(par - strokes, $"WTF +{par - strokes}");

			holeLabel.Text = $"Hole {hole}".ToUpper();

			(GolfHUD.Current as GolfHUD).Fade = true;
			AddClass("show");

			await Task.DelaySeconds(5);

			(GolfHUD.Current as GolfHUD).Fade = false;
			RemoveClass("show");
        }
	}

}