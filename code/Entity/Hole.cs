using Sandbox;

namespace Minigolf
{
	[Library("golf_hole")]
	public partial class GolfHole : ModelEntity
	{
		[HammerProp("hole")]
		public int Hole { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Always;
		}
	}
}
