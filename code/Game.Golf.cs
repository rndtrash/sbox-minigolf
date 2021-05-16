using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace Minigolf
{
	partial class GolfGame
	{
		[ServerVar( "minigolf_check_bounds" )]
		public static bool CheckBounds { get; set; } = true;

		public StaticCamera MapCamera = new StaticCamera();

        static readonly SoundEvent SoundHoleInOne = new SoundEvent("sounds/minigolf.crowd_ovation.vsnd");
		static readonly SoundEvent SoundBelowPar = new SoundEvent("sounds/minigolf.fart.vsnd");
		static readonly SoundEvent InHoleSound = new SoundEvent("sounds/minigolf.ball_inhole.vsnd");

		public void OnBallStoppedMoving(GolfBall ball)
		{
			if ( CheckBounds && !ball.Cupped && !Course.CurrentHole.InBounds(ball) )
				BallOutOfBounds(ball, OutOfBoundsType.Normal);
		}

		public enum OutOfBoundsType
		{
			Normal,
			Water,
			Fire
		}

		public void BallOutOfBounds(GolfBall ball, OutOfBoundsType type)
        {
			if ( IsClient )
				return;

			ResetBall( ball );

			// Tell the ball owner his balls are out of bounds
			ClientBallOutOfBounds( ball.Player, ball );
		}

		[ClientRpc]
		public void ClientBallOutOfBounds(GolfBall ball)
		{
			_ = OutOfBounds.Current.Show();
		}

		public void OnBallInHole(GolfBall ball, int hole)
        {
			var player = ball.Player;

			ball.Cupped = true;
			ball.PlaySound(InHoleSound.Name);

			// Announce to all players
			Sandbox.UI.ChatBox.AddInformation(Player.All, $"{player.Name} scored on hole {hole}!", $"avatar:{player.SteamId}");
			GolfBallInHole(ball, player.Strokes);

			Action task = async () =>
			{
				await Task.DelaySeconds(5);

				Course.AdvancedHole();

				// Reset for now
				player.Strokes = 0;
				ResetBall(ball);
			};
			task.Invoke();
		}

		[ClientRpc]
		protected void GolfBallInHole(GolfBall ball, int strokes)
        {
			// nice job bro, hole in one!
			if (strokes == 1)
				Sound.FromScreen(SoundHoleInOne.Name).SetVolume(0.8f);
			else if (strokes - Course.CurrentHole.Par > 0)
				Sound.FromScreen(SoundBelowPar.Name);

			_ = EndScore.Current.ShowScore(Course.CurrentHole.Number, Course.CurrentHole.Par, strokes);
		}

		protected void ResetBall(GolfBall ball)
		{
			ball.ResetPosition( Course.CurrentHole.SpawnPosition, Course.CurrentHole.SpawnAngles );
		}

		[ServerCmd("minigolf_stroke")]
		public static void GolfBallStroke(float yaw, float power)
		{
			var owner = ConsoleSystem.Caller;

			if (owner == null && owner is GolfPlayer)
				return;

			var player = owner as GolfPlayer;
			var ball = player.ActiveChild as GolfBall;

			if (ball.Stroke( Angles.AngleVector( new Angles( 0, yaw, 0 ) ), power ))
				player.Strokes++;
		}
	}
}
