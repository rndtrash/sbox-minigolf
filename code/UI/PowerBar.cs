
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace Minigolf
{
	public partial class PowerBar : Panel
	{
		public static PowerBar Current;

		private Panel bar;

		public PowerBar()
		{
			Current = this;

			StyleSheet.Load("/ui/PowerBar.scss");

			Add.Label("POWER");
			bar = Add.Panel("bar").Add.Panel();
		}

        public override void Tick()
        {
			var player = Player.Local as GolfPlayer;
			if (player == null) return;

			if (player.ShotPower > 0.0f)
            {
				AddClass("active");

				var length = new Length();
				length.Value = player.ShotPower;
				length.Unit = LengthUnit.Percentage;

				bar.Style.Width = length;
				bar.Style.Dirty();

				return;
            }

			RemoveClass("active");
        }
	}

}