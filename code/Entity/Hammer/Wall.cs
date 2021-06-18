using Sandbox;
using System;
using System.Threading.Tasks;

namespace Minigolf
{
	[Library("minigolf_wall", Description = "Wall stuff")]
	[Hammer.Solid]
	[Hammer.PhysicsTypeOverride( Hammer.PhysicsTypeOverrideAttribute.PhysicsTypeOverride.Mesh )]
	[Hammer.RenderProperties]
	public partial class Wall : ModelEntity
	{
		[Property( "Reflect", "If checked, the ball will bounce off this surface at the defined multiplier." )]
		public bool Reflect { get; set; } = true;

		[Property( "ReflectMultiplier", "Reflect multiplier", "How much the wall will reflect" )]
		public float ReflectMultiplier { get; set; } = 1;

		//  surface_property_override(surface_properties) : "Surface Property Override" : "" : "Overrides the default surface property."

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
