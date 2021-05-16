using Sandbox;

namespace Minigolf
{
	[Library( "minigolf_hole_camera" )]
	public partial class HoleCamera : ModelEntity
	{
		[HammerProp( "hole_number" )]
		public int Hole { get; set; }
		[HammerProp( "FOV" )]
		public float FOV { get; set; }
		[HammerProp( "ZNear" )]
		public float ZNear { get; set; }
		[HammerProp( "ZFar" )]
		public float ZFar { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Never;
		}
	}

	[Library( "minigolf_start_camera" )]
	public partial class StartCamera : ModelEntity
	{
		[HammerProp( "FOV" )]
		public float FOV { get; set; }
		[HammerProp( "ZNear" )]
		public float ZNear { get; set; }
		[HammerProp( "ZFar" )]
		public float ZFar { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			// lazy hack
			Transmit = TransmitType.Always;
		}
	}
}
