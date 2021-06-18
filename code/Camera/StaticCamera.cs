using Sandbox;
using System.Linq;

namespace Minigolf
{
    public class StaticCamera : ICamera
    {
		public Vector3 Position;
		public Rotation Rotation;
		public float FieldOfView;

        public StaticCamera()
        {
            Position = new Vector3(-1910, 744, 616);
            Rotation = Rotation.From(19, -60, 0);
            FieldOfView = 90;
        }

		public override void Build( ref CameraSetup camSetup )
		{
			camSetup.Position = Position;
			camSetup.Rotation = Rotation;
			camSetup.FieldOfView = FieldOfView;
		}

		public override void BuildInput( InputBuilder builder )
		{

		}
    }
}
