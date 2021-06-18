using Sandbox;
using System;

namespace Minigolf
{
	public partial class GolfBall
	{
		[ServerVar( "minigolf_power_multiplier" )]
		public static float PowerMultiplier { get; set; } = 1500.0f;

		[ServerVar( "minigolf_unlimited_whacks" )]
		public static bool UnlimitedWhacks { get; set; } = false;

		static readonly string[][] SwingSounds = new string[][] {
			new string[] {
				new("minigolf.swing_supersoft_01"),
				new("minigolf.swing_supersoft_02"),
				new("minigolf.swing_supersoft_03"),
			},
			new string[] {
				new("minigolf.swing_soft_01"),
				new("minigolf.swing_soft_02"),
				new("minigolf.swing_soft_03"),
			},
			new string[] {
				new("minigolf.swing_medium_01"),
				new("minigolf.swing_medium_02"),
				new("minigolf.swing_medium_03"),
			},
			new string[] {
				new("minigolf.swing_hard_01"),
				new("minigolf.swing_hard_02"),
				new("minigolf.swing_hard_03"),
			},
		};

		public bool Stroke( Vector3 direction, float power )
		{
			if ( Cupped || (!UnlimitedWhacks && Moving) )
				return false;

			direction = direction.Normal;
			power = Math.Clamp( power, 0, 1 );

			var sound = SwingSounds[(int)MathF.Ceiling(power / 25)][Rand.Int(0, 2)];
			Sound.FromWorld(sound, Position);

			// Make sure we don't jump up at all.
			direction.z = 0;

			PhysicsBody.Velocity = direction * power * PowerMultiplier;
			PhysicsBody.AngularVelocity = 0;
			PhysicsBody.Wake();

			return true;
		}

		[ServerCmd( "minigolf_stroke" )]
		public static void GolfBallStroke( float yaw, float power )
		{
			if ( ConsoleSystem.Caller.Pawn is not GolfBall ball )
				return;

			ball.Stroke( Angles.AngleVector( new Angles( 0, yaw, 0 ) ), power );
		}
	}
}
