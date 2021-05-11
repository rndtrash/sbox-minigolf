using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minigolf
{
	[Library( "minigolf_flag_base" )]
	public partial class FlagBase : ModelEntity
	{
		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/golf_flag.vmdl" );

			MoveType = MoveType.None;
			CollisionGroup = CollisionGroup.Never;
			PhysicsEnabled = false;
			UsePhysicsCollision = false;

			Transmit = TransmitType.Always;
		}
	}
}
