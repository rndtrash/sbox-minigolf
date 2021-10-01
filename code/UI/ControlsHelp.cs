
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

			AddEntry( "keyboard", "Other Bullshit" );
			AddEntry( "mouse", "Move Camera" );
			AddEntry( "keyboard", Input.GetKeyWithBinding( "+iv_score" ) );			
		}

		protected void AddEntry(string icon, string name)
		{
			var row = Add.Panel( "row" );
			row.Add.Icon( icon, "icon" );
			row.Add.Label( name, "text" );
		}
	}

}
