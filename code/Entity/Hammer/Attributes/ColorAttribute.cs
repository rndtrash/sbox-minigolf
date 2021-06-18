using Sandbox;
using System;
using System.Text;

namespace Hammer
{
	/// <summary>
	/// Change the color of the wireframe box in the Hammer 2D views.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class )]
	public class ColorAttribute : MetaDataAttribute
	{
		internal byte ColorR;
		internal byte ColorG;
		internal byte ColorB;

		public ColorAttribute( byte r, byte g, byte b )
		{
			ColorR = r;
			ColorG = g;
			ColorB = b;
		}

		public override void AddHeader( StringBuilder sb )
		{
			sb.Append( $" color( {ColorR} {ColorG} {ColorB} )" );
		}
	}
}
