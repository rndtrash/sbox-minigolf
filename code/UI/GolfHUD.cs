using Sandbox;
using Sandbox.UI;

namespace Minigolf
{
	[Library]
	public partial class GolfHUD : Hud
	{
		public GolfHUD()
		{
			if (!IsClient) return;

			RootPanel.AddChild<Sandbox.UI.ChatBox>();
			RootPanel.AddChild<PowerBar>();
		}
	}
}