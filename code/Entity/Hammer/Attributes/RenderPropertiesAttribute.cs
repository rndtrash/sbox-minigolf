using Sandbox;
using System;
using System.Text;

namespace Hammer
{
	/// <summary>
	/// Use this to add Render Properties to the entity, these are used by the map builder.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class )]
	public class RenderPropertiesAttribute : MetaDataAttribute
	{
		public override void AddHeader( StringBuilder sb )
		{
			// todo: multiple bases
			sb.Append( $" base( RenderFields ) " );
		}

		public override void AddBody( StringBuilder sb )
		{
			// vrad_brush_cast_shadows is used internally by the map builder on brushes
			sb.AppendLine( "\tvrad_brush_cast_shadows( boolean )[ group=\"Render Properties\" ] : \"Shadows\" : 0 : \"Set this if this brush casts lightmap shadows.\"" );

			// equally not covered in RenderFields base but still used by the map builder
			sb.AppendLine( "\tdisableshadows( boolean )[group = \"Render Properties\"] : \"Disable shadows\" : 0" );
		}
	}
}
