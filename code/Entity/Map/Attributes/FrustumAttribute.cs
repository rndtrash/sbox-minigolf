using Sandbox;
using System;
using System.Text;

namespace Hammer
{
	[AttributeUsage( AttributeTargets.Class )]
	public class FrustumAttribute : MetaDataAttribute
	{
		internal string FOVField;
		internal string NearField;
		internal string FarField;

		public FrustumAttribute( string fovField, string nearField, string farField )
		{
			FOVField = fovField;
			NearField = nearField;
			FarField = farField;
		}

		public override void AddHeader( StringBuilder sb )
		{
			sb.Append( $" frustum( {FOVField}, {NearField}, {FarField} )" );
		}
	}
}
