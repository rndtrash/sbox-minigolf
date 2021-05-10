using Sandbox;

namespace Minigolf
{
	[Library("minigolf_hole_spawn")]
	public partial class HoleSpawn : ModelEntity
	{
		[HammerProp("hole_number")]
		public int Hole { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Never;
		}
	}
}
