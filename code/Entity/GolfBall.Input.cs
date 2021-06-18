using Sandbox;
using System;

namespace Minigolf
{
	public partial class GolfBall
	{
		public float ShotPower = 0.0f;

		public override void BuildInput( InputBuilder input )
		{
			Host.AssertClient();

			if ( Camera is not FollowBallCamera camera )
				return;

			camera.BuildInput( input );

			// Okay maybe this should be a controller or something?
			if ( input.Down( InputButton.Attack1 ) )
			{
				ShotPower = Math.Clamp( ShotPower - input.AnalogLook.pitch, 0, 100 );
			}

			if ( ShotPower > 0.0f && !input.Down( InputButton.Attack1 ) )
			{
				ConsoleSystem.Run( "minigolf_stroke", camera.Angles.yaw, ShotPower / 100.0f );
				ShotPower = 0;
			}

			base.BuildInput( input );
		}
	}
}
