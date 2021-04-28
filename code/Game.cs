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

		[ServerCmd("golf_shoot")]
		public static void GolfShoot( float yaw, float power )
		{
			var owner = ConsoleSystem.Caller;

			if (owner == null)
				return;

			var angles = new Angles(0, yaw, 0);

			var player = owner as GolfPlayer;
			player.Ball.Velocity += Angles.AngleVector(angles) * power * 25.0f;

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
		}
	}
}