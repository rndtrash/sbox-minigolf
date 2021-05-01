using System;
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

		private Label bigLabel;
		private Label messageLabel;

		public StartingGame()
		{
			Current = this;

			StyleSheet.Load("/ui/StartingGame.scss");

			bigLabel = Add.Label("Game starting...", "big");
			messageLabel = Add.Label();
		}

		public void Hide()
        {
			(GolfHUD.Current as GolfHUD).Fade = false;
			RemoveClass("hide");
		}

        public override void Tick()
        {
			var game = Sandbox.Game.Current as GolfGame;
			if (!game.WaitingToStart)
				AddClass("hide");

			bigLabel.Text = "Game starting...";
			messageLabel.Text = $"< {TimeSpan.FromSeconds(Math.Clamp(Math.Ceiling(game.StartTime - Sandbox.Time.Now), 0, 300)).ToString(@"mm\:ss")} > ";
		}
    }

}