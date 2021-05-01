using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Minigolf
{
	[Library]
	public partial class GolfHUD : Hud
	{
		private bool _fade;
		public bool Fade {
			get { return _fade; }
			set {
				if (value)
					RootPanel.AddClass("fade");
				else
					RootPanel.RemoveClass("fade");

				_fade = value;
			}
		}

		public GolfHUD()
		{
			if (!IsClient) return;

			RootPanel.StyleSheet.Load("/ui/GolfHUD.scss");

			RootPanel.AddChild<Sandbox.UI.ChatBox>();
			RootPanel.AddChild<Sandbox.UI.Scoreboard<Sandbox.UI.ScoreboardEntry>>();
			RootPanel.AddChild<PowerBar>();
			RootPanel.AddChild<HoleScore>();
			RootPanel.AddChild<EndScore>();
			RootPanel.AddChild<OutOfBounds>();
			RootPanel.AddChild<StartingGame>();

			RootPanel.AddChild<NameTags>();

			RootPanel.Add.Label("Work In Progress", "wip");
		}
	}
}