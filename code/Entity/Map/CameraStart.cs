using Sandbox;

namespace Minigolf
{
	[Library( "minigolf_hole_camera", Description = "Minigolf Hole Camera" )]
	[Hammer.EditorModel( "models/editor/camera.vmdl" )]
	public partial class HoleCamera : Entity
	{
		[Property( "hole_number", "Hole Number", "Which hole this camera is for" )]
		public int Hole { get; set; } = 1;

		[Property( "FOV", "Field of view in degrees" )]
		public float FOV { get; set; } = 90.0f;
		[Property( "ZNear", "Distance to the near plane" )]
		public float ZNear { get; set; } = 4.0f;
		[Property( "ZFar", "Distance to the far plane" )]
		public float ZFar { get; set; } = 10000.0f;
	}

	[Library( "minigolf_start_camera", Description = "Minigolf Start Camera" )]
	[Hammer.EditorModel( "models/editor/camera.vmdl" )]
	public partial class StartCamera : Entity
	{
		public float FOV { get; set; } = 90.0f;
		[Property( "ZNear", "Distance to the near plane" )]
		public float ZNear { get; set; } = 4.0f;
		[Property( "ZFar", "Distance to the far plane" )]
		public float ZFar { get; set; } = 10000.0f;

		public override void Spawn()
		{
			base.Spawn();

			// lazy hack
			Transmit = TransmitType.Always;
		}
	}
}
