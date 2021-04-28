using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Minigolf
{
	[Library("minigolf", Title = "Minigolf")]
	partial class GolfGame : Sandbox.Game
	{
		public GolfGame()
		{
			// easy way for now.. todo look into actual clientside huds?
			if (IsServer)
				new GolfHUD();
		}

		public override Player CreatePlayer() => new GolfPlayer();

		public Vector3 FindBallSpawn(int hole)
        {
			var spawn = Entity.All.OfType<BallSpawn>().Where(x => x.Hole == hole).FirstOrDefault();

			// TODO: Trace to find a valid Up coord

			return spawn != null ? spawn.WorldPos : Vector3.Zero;
        }

		[ServerCmd("golf_shoot")]
		public static void GolfShoot( float yaw, float power )
		{
			var owner = ConsoleSystem.Caller;

			if (owner == null)
				return;

			var angles = new Angles(0, yaw, 0);

			var player = owner as GolfPlayer;
			player.Ball.Velocity += Angles.AngleVector(angles) * power * 25.0f;

			// remove when SoundEvents aren't fucked
			var modifier = "";
			if (power < 25.0f)
				modifier = "supersoft";
			else if (power < 50.0f)
				modifier = "soft";
			else if (power < 75.0f)
				modifier = "medium";
			else
				modifier = "hard";

			player.Ball.PlaySound($"golfswing_{modifier}_0{Rand.Int(1, 3)}");

			// var sound = SwingSounds[(int)MathF.Ceiling(power / 25)][Rand.Int(0, 2)];
			// player.Ball.PlaySound(sound.Name);
		}

		static readonly SoundEvent[][] SwingSounds = new SoundEvent[][] {
            new SoundEvent[] {
				new("sounds/golfswing_supersoft_01.vsnd"),
				new("sounds/golfswing_supersoft_02.vsnd"),
				new("sounds/golfswing_supersoft_03.vsnd"),
			},
			new SoundEvent[] {
				new("sounds/golfswing_soft_01.vsnd"),
				new("sounds/golfswing_soft_02.vsnd"),
				new("sounds/golfswing_soft_03.vsnd"),
			},
			new SoundEvent[] {
				new("sounds/golfswing_medium_01.vsnd"),
				new("sounds/golfswing_medium_02.vsnd"),
				new("sounds/golfswing_medium_03.vsnd"),
			},
			new SoundEvent[] {
				new("sounds/golfswing_hard_01.vsnd"),
				new("sounds/golfswing_hard_02.vsnd"),
				new("sounds/golfswing_hard_03.vsnd"),
			},
		};
	}
}