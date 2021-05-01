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

			Course = new Course();
			Course.LoadFromMap();
			foreach (var hole in Course.Holes)
				Log.Info($"[{hole.Key}] {hole.Value.Name} (Par {hole.Value.Par}) (Spawn: {hole.Value.SpawnPosition}) (Bounds {hole.Value.Bounds.Count})");
			}

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

			var balls = Entity.All.OfType<PlayerBall>();
			foreach(var ball in balls)
            {
				var wasMoving = ball.IsMoving;
				ball.IsMoving = !ball.Velocity.IsNearlyZero();

				if (ball.IsMoving == false && wasMoving == true)
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