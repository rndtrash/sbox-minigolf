using System;
using Sandbox;

namespace Minigolf
{
	public partial class Game
	{
		[Net] public Course Course { get; set; }

		[ServerVar( "minigolf_check_bounds" )]
		public static bool CheckBounds { get; set; } = true;

        static readonly SoundEvent SoundHoleInOne = new("sounds/minigolf.crowd_ovation.vsnd");
		static readonly SoundEvent SoundBelowPar = new("sounds/minigolf.fart.vsnd");
		static readonly SoundEvent InHoleSound = new("sounds/minigolf.ball_inhole.vsnd");

		public void OnBallStoppedMoving(Ball ball)
		{
			// if ( CheckBounds && !ball.Cupped && !Course.CurrentHole.InBounds(ball) )
			// 	BallOutOfBounds(ball, OutOfBoundsType.Normal);
		}

		public enum OutOfBoundsType
		{
			Normal,
			Water,
			Fire
		}

		public void BallOutOfBounds(Ball ball, OutOfBoundsType type)
        {
			if ( IsClient )
				return;

			ResetBall( ball );

			// Tell the ball owner his balls are out of bounds
			ClientBallOutOfBounds( To.Single(ball) );
		}

		[ClientRpc]
		public void ClientBallOutOfBounds()
		{
			_ = OutOfBounds.Current.Show();
		}

		/// <summary>
		/// Called from the HoleGoal entity 
		/// </summary>
		/// <param name="ball"></param>
		/// <param name="hole"></param>
		public void CupBall( Ball ball, int hole )
        {
			if ( IsClient ) return;

			// Make sure the hole they cupped in is the current one...
			if ( hole != Course.CurrentHole.Number )
			{
				// Do a custom reset if you cup it in the wrong hole
				ResetBall( ball );

				return;
			}

			// Cup the ball entity, this does fx and stops motion.
			ball.Cup();

			// Let all players know the ball has been cupped.
			var score = ball.Client.Components.GetOrCreate<ScoreComponent>();
			CuppedBall( To.Everyone, ball, score.Score );

			// ball.DeleteAsync( 5.0f );

			// Update their score?
			// Delete their ball?

			Action task = async () =>
			{
				await Task.DelaySeconds(5);

				Course.NextHole();
				// Course.AdvancedHole();

				// Reset for now
				// player.Strokes = 0;
				ResetBall(ball);
			};
			task.Invoke();
		}

		[ClientRpc]
		protected void CuppedBall( Ball ball, int score )
		{
			var client = ball.Client;

			// Add to UI ( This should be our "kill" feed )
			Minigolf.ChatBox.AddInformation($"{client.Name} scored on hole {Course.CurrentHole.Number}!", $"avatar:{client.SteamId}");

			if ( Local.Client != client ) return;

			_ = EndScore.Current.ShowScore( Course.CurrentHole.Number, Course.CurrentHole.Par, score );
		}

		protected void ResetBall(Ball ball)
		{
			if ( IsClient )
				return;

			ball.ResetPosition( Course.CurrentHole.SpawnPosition, Course.CurrentHole.SpawnAngles );
		}

		// fuck it do this somewhere else and keep score?
		[ServerCmd]
		public static void Stroke( float yaw, float power )
		{
			var client = ConsoleSystem.Caller;
			if ( client == null ) return;

			if ( ConsoleSystem.Caller.Pawn is not Ball ball )
				return;

			if ( ball.InPlay )
				return;

			var score = client.Components.GetOrCreate<ScoreComponent>();
			score.Score += 1;

			ball.Stroke( Angles.AngleVector( new Angles( 0, yaw, 0 ) ), power );
		}
	}
}
