using Sandbox;
using System;
using System.Threading.Tasks;

namespace Minigolf
{
	[Library("minigolf_wall")]
	public partial class Wall : ModelEntity
	{
		[HammerProp( "Reflect" )]
		public bool Reflect { get; set; }

		[HammerProp( "ReflectMultiplier" )]
		public float ReflectMultiplier { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			EnableDrawing = true;

			SetupPhysicsFromModel( PhysicsMotionType.Static );

			// Custom surface property with 0 friction, 0 elasticity.
			PhysicsGroup.SetSurface( "minigolf.side" );

			Transmit = TransmitType.Always;
		}
	}
}
