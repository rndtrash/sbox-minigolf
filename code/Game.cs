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
				ball.IsMoving = ball.Velocity.Length > 0.1f;

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