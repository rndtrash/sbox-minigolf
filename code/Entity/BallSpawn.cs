using Sandbox;

namespace Minigolf
{
	[Library("minigolf_ball_spawn")]
	public partial class BallSpawn : ModelEntity
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
