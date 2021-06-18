
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace Minigolf
{
	public partial class ControlsHelp : Panel
	{
		public ControlsHelp()
		{
			StyleSheet.Load( "/ui/ControlsHelp.scss" );

			AddEntry( "mouse", "Move Camera" );
		}

		protected void AddEntry(string icon, string name)
		{

		}
	}

}
