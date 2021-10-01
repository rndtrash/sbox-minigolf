using Sandbox;
using Sandbox.UI;

namespace Minigolf
{
	[UseTemplate]
	public partial class PowerBar : Panel
	{
		Panel Bar { get; set; }

		public override void Tick()
		{
			if ( Local.Pawn is not Ball ball ) return;

			Bar.Style.Width = Length.Percent( ball.ShotPower * 100 );
			Bar.Style.Dirty();
		}
	}

}
