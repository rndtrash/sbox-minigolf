using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minigolf
{
	public partial class OutOfBounds : Panel
	{
		public static OutOfBounds Current;

		public OutOfBounds()
		{
			Current = this;

			StyleSheet.Load("/ui/OutOfBounds.scss");

			Add.Label("OUT OF BOUNDS", "big");
			Add.Label("YOU STUPID TWAT");
		}

		public async Task Show()
		{
			// choose a random string for second label

			(GolfHUD.Current as GolfHUD).Fade = true;
			AddClass("show");

			await Task.DelaySeconds(3);

			(GolfHUD.Current as GolfHUD).Fade = false;
			RemoveClass("show");
		}
	}

}