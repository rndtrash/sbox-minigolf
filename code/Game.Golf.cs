using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace Minigolf
{
	partial class GolfGame
	{
		[ServerVar("minigolf_power_multiplier")]
		public static float PowerMultiplier { get; set; } = 25.0f;

		// todo: move this stuff out!!
		[Net] public int CurrentHole { get; set; } = 1;
		public int HolePar { get; set; } = 2;

		[Net] public bool WaitingToStart { get; set; } = true;

		static readonly SoundEvent SoundHoleInOne = new SoundEvent("sounds/minigolf.crowd_ovation.vsnd");
		static readonly SoundEvent SoundBelowPar = new SoundEvent("sounds/minigolf.fart.vsnd");
		static readonly SoundEvent InHoleSound = new SoundEvent("sounds/minigolf.ball_inhole.vsnd");

		static readonly SoundEvent[][] SwingSounds = new SoundEvent[][] {
			new SoundEvent[] {
				new("sounds/minigolf.swing_supersoft_01.vsnd"),
				new("sounds/minigolf.swing_supersoft_02.vsnd"),
				new("sounds/minigolf.swing_supersoft_03.vsnd"),
			},
			new SoundEvent[] {
				new("sounds/minigolf.swing_soft_01.vsnd"),
				new("sounds/minigolf.swing_soft_02.vsnd"),
				new("sounds/minigolf.swing_soft_03.vsnd"),
			},
			new SoundEvent[] {
				new("sounds/minigolf.swing_medium_01.vsnd"),
				new("sounds/minigolf.swing_medium_02.vsnd"),
				new("sounds/minigolf.swing_medium_03.vsnd"),
			},
			new SoundEvent[] {
				new("sounds/minigolf.swing_hard_01.vsnd"),
				new("sounds/minigolf.swing_hard_02.vsnd"),
				new("sounds/minigolf.swing_hard_03.vsnd"),
			},
		};

		/// <summary>
		/// Reset's the ball to the spawn point of the current hole.
		/// </summary>
		/// <param name="ball"></param>
		public void ResetBall(PlayerBall ball)
        {
			var spawn = Entity.All.OfType<HoleSpawn>().Where(x => x.Hole == CurrentHole).FirstOrDefault();
			if (spawn == null) return;

			// todo: trace up

			// Reset all velocity
			ball.PhysicsBody.Velocity = Vector3.Zero;
			ball.PhysicsBody.AngularVelocity = Vector3.Zero;

			ball.WorldPos = spawn.WorldPos;

			ball.IsMoving = false;
			ball.InHole = false;
		}

		public void OnBallStoppedMoving(PlayerBall ball)
		{
			if (!ball.InHole && !HoleInfo.InBounds(CurrentHole, ball))
				BallOutOfBounds(ball);
		}

		public void BallOutOfBounds(PlayerBall ball)
        {
			ResetBall(ball);

			// Tell the ball owner his balls are out of bounds
			ClientBallOutOfBounds(ball.Owner, ball);
		}

		[ClientRpc]
		public void ClientBallOutOfBounds(PlayerBall ball)
		{
			_ = OutOfBounds.Current.Show();
		}

		public void OnBallInHole(PlayerBall ball, int hole)
        {
			var player = ball.Owner as GolfPlayer;

			ball.InHole = true;
			ball.PlaySound(InHoleSound.Name);

			// Announce to all players
			Sandbox.UI.ChatBox.AddInformation(Player.All, $"{player.Name} scored on hole {hole}!", $"avatar:{player.SteamId}");
			PlayerBallInHole(ball, player.Strokes);

			Action task = async () =>
			{
				await Task.DelaySeconds(5);

				CurrentHole = CurrentHole == 1 ? 2 : 1;

				// Reset for now
				player.Strokes = 0;
				ResetBall(ball);
			};
			task.Invoke();
		}

		[ClientRpc]
		protected void PlayerBallInHole(PlayerBall ball, int strokes)
        {
			// nice job bro, hole in one!
			if (strokes == 1)
				Sound.FromScreen(SoundHoleInOne.Name).SetVolume(1.5f);
			else if (strokes - HolePar > 0)
				Sound.FromScreen(SoundBelowPar.Name);

			_ = EndScore.Current.ShowScore(CurrentHole, HolePar, strokes);
		}

		[ServerCmd("minigolf_stroke")]
		public static void PlayerBallStroke(float yaw, int power)
		{
			var owner = ConsoleSystem.Caller;

			if (owner == null && owner is GolfPlayer)
				return;

			var player = owner as GolfPlayer;

			// Don't let a player hit an already moving ball or one in the hole
			// if (player.Ball.IsMoving || player.Ball.InHole)
			// 	return;

			// Clamp the power, should be 0-100
			power = Math.Clamp(power, 0, 100);

			// remove when SoundEvents aren't fucked
			string modifier;
			if (power < 25)
				modifier = "supersoft";
			else if (power < 50)
				modifier = "soft";
			else if (power < 75)
				modifier = "medium";
			else
				modifier = "hard";

			// Smack that ball
			player.Ball.PhysicsBody.Velocity += Angles.AngleVector(new Angles(0, yaw, 0)) * (float)power * PowerMultiplier;
			player.Ball.PhysicsBody.AngularVelocity = Vector3.Zero;

			// Play the sound from where the ball was, the sound shouldn't follow the ball
			Sound.FromWorld($"minigolf.swing_{modifier}_0{Rand.Int(1, 3)}", player.Ball.WorldPos);

			// var sound = SwingSounds[(int)MathF.Ceiling(power / 25)][Rand.Int(0, 2)];
			// Sound.FromWorld(sound.Name, player.Ball.WorldPos);

			player.Strokes++;
		}
	}
}