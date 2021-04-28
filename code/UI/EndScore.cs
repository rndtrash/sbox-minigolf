
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace Minigolf
{
	public partial class EndScore : Panel
	{
		public static EndScore Current;

		public bool Show;

		private Label score;
		private Label hole;

		public EndScore()
		{
			Current = this;

			StyleSheet.Load("/ui/EndScore.scss");

			score = Add.Label("BOGEY", "score");
			hole = Add.Label("HOLE 1", "hole");
		}

		public override void Tick()
		{
			Show = Player.Local.Input.Down(InputButton.Jump);

			if (!Show)
            {
				(GolfHUD.Current as GolfHUD).Fade = false;
				RemoveClass("show");
				return;
            }

			AddClass("show");
			// when we are showing, make sure the rootpanel is faded, probably a better way to do this
			(GolfHUD.Current as GolfHUD).Fade = true;
		}
	}

}