using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Minigolf
{
	[Library("minigolf", Title = "Minigolf")]
	partial class GolfGame : Sandbox.Game
	{
		[ServerVar("minigolf_power_multiplier")]
		public static float PowerMultiplier { get; set; } = 25.0f;


		[Net] public int CurrentHole { get; set; } = 1;

		public int HolePar { get; set; } = 2;

		public GolfGame()
		{
			// easy way for now.. todo look into actual clientside huds?
			if (IsServer)
				new GolfHUD();

			_ = StartTickTimer();
		}

		public override Player CreatePlayer() => new GolfPlayer();

		public async Task StartTickTimer()
		{
			while (true)
			{
				await Task.NextPhysicsFrame();
				OnTick();
			}
		}

		public void OnTick()
        {
			if (Host.IsClient)
				return;

			var balls = Entity.All.OfType<PlayerBall>();
			foreach(var ball in balls)
            {
				var wasMoving = ball.IsMoving;
				ball.IsMoving = ball.Velocity.Length != 0.0f;

				if (ball.IsMoving == false && wasMoving == true)
					OnBallStoppedMoving(ball);
			}
		}

		public void OnBallStoppedMoving(PlayerBall ball)
        {
			if (HoleInfo.InBounds(CurrentHole, ball))
				return;

			BallOutOfBounds(ball.Owner, ball);
			ball.WorldPos = FindBallSpawn(CurrentHole);
		}

		[ClientRpc]
		public void BallOutOfBounds(PlayerBall ball)
        {
			_ = OutOfBounds.Current.Show();
		}

		public Vector3 FindBallSpawn(int hole)
        {
			var spawn = Entity.All.OfType<BallSpawn>().Where(x => x.Hole == hole).FirstOrDefault();

			// TODO: Trace to find a valid Up coord

			return spawn != null ? spawn.WorldPos : Vector3.Zero;
        }

		public void OnBallInHole(PlayerBall ball, int hole)
        {
			var player = ball.Owner;
			Sandbox.UI.ChatBox.AddInformation(Player.All, $"{player.Name} putted on hole {hole}!", $"avatar:{player.SteamId}");
			ball.PlaySound(PuttSound.Name);

			_ = DoBallStuff(ball, hole);
		}

		public async Task DoBallStuff(PlayerBall ball, int hole)
		{
			var strokes = (ball.Owner as GolfPlayer).Strokes;
			DoBallClientSide(ball.Owner, ball, hole, HolePar, strokes);

			await Task.DelaySeconds(5);

			if (CurrentHole == 1)
				CurrentHole = 2;
			else
				CurrentHole = 1;

			(ball.Owner as GolfPlayer).Strokes = 0;
			ball.WorldPos = FindBallSpawn(CurrentHole);
		}

		[ClientRpc]
		public void DoBallClientSide(PlayerBall ball, int hole, int par, int strokes)
        {
			Host.AssertClient();
			_ = EndScore.Current.ShowScore(hole, par, strokes);
        }

		[ServerCmd("golf_shoot")]
		public static void GolfShoot( float yaw, float power )
		{
			var owner = ConsoleSystem.Caller;

			if (owner == null)
				return;

			// scale the power
			var velocityPower = power * PowerMultiplier;

			var angles = new Angles(0, yaw, 0);

			var player = owner as GolfPlayer;
			player.Ball.PhysicsBody.Velocity += Angles.AngleVector(angles) * velocityPower;
			player.Ball.PhysicsBody.AngularVelocity = Vector3.Zero;

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

			player.Strokes++;

			// var sound = SwingSounds[(int)MathF.Ceiling(power / 25)][Rand.Int(0, 2)];
			// player.Ball.PlaySound(sound.Name);
		}

		public override void DoPlayerDevCam(Player player)
		{
			if (!player.HasPermission("devcam"))
				return;

			if (player is GolfPlayer basePlayer)
			{
				if (basePlayer.DevCamera is DevCamera)
				{
					basePlayer.DevCamera = null;
				}
				else
				{
					basePlayer.DevCamera = new DevCamera();
				}
			}
		}

		static readonly SoundEvent PuttSound = new SoundEvent("sounds/ballinhole.vsnd");

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