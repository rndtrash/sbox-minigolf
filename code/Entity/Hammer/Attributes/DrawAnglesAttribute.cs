using Sandbox;
using System;
using System.Text;

namespace Hammer
{
	/// <summary>
	/// Draw the entity's transform angle or specific angles property in Hammer.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class )]
	public class DrawAnglesAttribute : MetaDataAttribute
	{
		internal string AngleProperty;
		internal string LocalProperty;

		public DrawAnglesAttribute() { }

		public DrawAnglesAttribute( string property )
		{
			AngleProperty = property;
		}

		public DrawAnglesAttribute( string property, string local )
		{
			AngleProperty = property;
			LocalProperty = local;
		}

		public override void AddHeader( StringBuilder sb )
		{
			// drawangles()
			// drawangles(angle)
			// drawangles(angle, local)

			sb.Append( " drawangles( " );
			
			if ( !string.IsNullOrEmpty( AngleProperty ) )
			{
				sb.Append( $"{StringX.QuoteSafe( AngleProperty )}" );

				if ( !string.IsNullOrEmpty( LocalProperty ) )
					sb.Append( $", {StringX.QuoteSafe( LocalProperty )}" );
			}

			sb.Append( " ) " );
		}
	}
}
