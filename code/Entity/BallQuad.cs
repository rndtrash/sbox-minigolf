using Sandbox;

namespace Minigolf
{
	public partial class BallQuad : RenderEntity
	{
		public Material Material = Material.Load( "materials/minigolf.ball_circle.vmat" );

		public Vector3 Direction = Vector3.Zero;
		public bool ShouldDraw = false;

		public override void DoRender( SceneObject obj )
		{
			if ( !ShouldDraw )
				return;

			Render.SetLighting( obj );

			var vertexBuffer = Render.GetDynamicVB( true );

			vertexBuffer.AddQuad( new Ray( WorldPos, Vector3.Up ), Vector3.Forward * 8, Vector3.Left * 8 );

			vertexBuffer.Draw( Material );
		}
	}
}
