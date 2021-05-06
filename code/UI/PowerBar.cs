
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

				var normalizedShotPower = player.ShotPower / 100;
				var squared = normalizedShotPower * normalizedShotPower;

				var color = ColorConvert.HSLToRGB( 120 - (int)(squared * 120), 1.0f, 0.5f );
				// var color = new Color(normalizedShotPower, 1 - normalizedShotPower, 0);

				bar.Style.BackgroundColor = color;
				bar.Style.Width = length;
				bar.Style.Dirty();

				return;
            }

			RemoveClass("active");
        }
	}

}
