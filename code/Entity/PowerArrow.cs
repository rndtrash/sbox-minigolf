using Sandbox;

namespace Minigolf
{
	public partial class PowerArrow : RenderEntity
	{
		public Material Material = Material.Load( "materials/minigolf.arrow.vmat" );

		public Vector3 Direction = Vector3.Zero;
		public float Power = 0.0f;

		public override void DoRender( SceneObject obj )
		{
			if ( Power == 0.0f )
				return;

			Render.SetLighting( obj );

			var vertexBuffer = Render.GetDynamicVB(true);

			var startPos = WorldPos;
			var endPos = WorldPos += Direction * Power * 100;
			var offset = Vector3.Cross( Direction, Vector3.Up ) * (1 + 2 * Power);

			// Line
			Vertex a = new( startPos - offset, Vector3.Up, Vector3.Right, new Vector4( 0, 1, 0, 0 ) );
			Vertex b = new( startPos + offset, Vector3.Up, Vector3.Right, new Vector4( 1, 1, 0, 0 ) );
			Vertex c = new( endPos + offset, Vector3.Up, Vector3.Right, new Vector4( 1, 0, 0, 0 ) );
			Vertex d = new( endPos - offset, Vector3.Up, Vector3.Right, new Vector4( 0, 0, 0, 0 ) );

			vertexBuffer.Add( a );
			vertexBuffer.Add( b );
			vertexBuffer.Add( c );
			vertexBuffer.Add( d );

			vertexBuffer.AddTriangleIndex( 4, 3, 2 );
			vertexBuffer.AddTriangleIndex( 2, 1, 4 );

			// Add the arrow tip
			Vertex e = new( endPos + offset * 2, Vector3.Up, Vector3.Right, new Vector4( 1, 0, 0, 0 ) );
			Vertex f = new( endPos - offset * 2, Vector3.Up, Vector3.Right, new Vector4( 0, 0, 0, 0 ) );
			Vertex g = new( endPos + Direction * 16 * Power, Vector3.Up, Vector3.Right, new Vector4( 1, 0, 0, 0 ) );

			vertexBuffer.Add( e );
			vertexBuffer.Add( f );
			vertexBuffer.Add( g );
			vertexBuffer.AddTriangleIndex( 1, 2, 3 );

			vertexBuffer.Draw( Material );
		}
	}
}
