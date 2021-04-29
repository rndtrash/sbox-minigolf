using Sandbox;

namespace Minigolf
{
	[Library("minigolf_hole_info")]
	public partial class HoleInfoEntity : ModelEntity
	{
		[HammerProp("hole")]
		public int Hole { get; set; }
		[HammerProp("name")]
		public string Name { get; set; }
		[HammerProp("par")]
		public int Par { get; set; }


		public override void Spawn()
		{
			base.Spawn();
		}
	}
}
