﻿using Sandbox;
using System;

namespace Minigolf
{
	public partial class Ball
	{
		/// <summary>
		/// The current shot power...
		/// </summary>
		public float ShotPower { get; set; } = 0.0f;
		public float LastShotPower { get; set; } = 0.0f;

		public override void BuildInput( InputBuilder input )
		{
			// If we're in play, don't do anything.
			if ( InPlay )
				return;

			if ( input.Down( InputButton.Attack1 ) )
			{
				float delta = input.AnalogLook.pitch * Time.Delta;
				ShotPower = Math.Clamp( ShotPower - delta, 0, 1 );
			}

			if ( ShotPower >= 0.01f && !input.Down( InputButton.Attack1 ) )
			{
				Game.Stroke( Game.Current.BallCamera.Angles.yaw, ShotPower );
				LastShotPower = ShotPower;
				ShotPower = 0;
			}
		}
	}
}
