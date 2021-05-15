using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace Minigolf
{
	[Library("minigolf", Title = "Minigolf")]
	partial class GolfGame : Sandbox.Game
	{
		[Net] public bool WaitingToStart { get; set; } = true;
		[Net] public float StartTime { get; set; }

		[Net] public Course Course { get; set; }

		public GolfGame()
		{
			// easy way for now.. todo look into actual clientside huds?
			if (IsServer)
            {
				new GolfHUD();
				Course = new();
			}
		}

		public override Player CreatePlayer() => new GolfPlayer();

		public override void PostLevelLoaded()
		{
			WaitingToStart = true;
			StartTime = (float)Math.Floor(Time.Now + 5.0f);

			Course = new Course();
			Course.LoadFromMap();
			foreach (var hole in Course.Holes)
				Log.Info($"[{hole.Key}] {hole.Value.Name} (Par {hole.Value.Par}) (Spawn: {hole.Value.SpawnPosition}) (Bounds {hole.Value.Bounds.Count})");
		}

		[Event( "tick" )]
		public void OnTick()
        {
			if (Host.IsClient)
            {
				if (Course == null || Course.CurrentHole == null)
					return;

				DebugOverlay.ScreenText($"Course.CurrentHole: Hole {Course.CurrentHole.Number} - {Course.CurrentHole.Name} (Par {Course.CurrentHole.Par})");

				foreach (var hole in Course.Holes)
					DebugOverlay.ScreenText(hole.Key, $"[{hole.Key}]: {hole.Value.Name} (Par {hole.Value.Par})");

				return;
			}

			if (Time.Now > StartTime)
				WaitingToStart = false;

			var balls = Entity.All.OfType<GolfBall>();
			foreach(var ball in balls)
            {
				var wasMoving = ball.Moving;
				ball.Moving = !ball.Velocity.IsNearlyZero();

				if (ball.Moving == false && wasMoving == true)
					OnBallStoppedMoving(ball);
			}
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
	}
}
