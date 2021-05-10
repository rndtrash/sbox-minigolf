using Sandbox;

namespace Minigolf
{
	[Library("minigolf_hole_spawn")]
	public partial class HoleSpawn : ModelEntity
	{
		[HammerProp( "hole_number" )]
		public int Number { get; set; }

		[HammerProp( "hole_name" )]
		public string Name { get; set; }

		[HammerProp( "hole_par" )]
		public int Par { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Never;
		}
	}
}
