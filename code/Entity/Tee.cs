using Sandbox;

namespace Minigolf
{
	[Library("golf_tee")]
	public partial class GolfTee : ModelEntity
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
