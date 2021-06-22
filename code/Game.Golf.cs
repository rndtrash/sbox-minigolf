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
		public void CupBall( GolfBall ball, int hole )
        {
			// Make sure the hole they cupped in is the current one...
			if ( hole != Course.CurrentHole.Number )
			{
				// Do a custom reset if you cup it in the wrong hole
				ResetBall( ball );

				return;
			}

			// Cup the ball entity, this does fx and stops motion.
			ball.Cup( false );

			// Let all players know the ball has been cupped.
			// CuppedBall( To.Everyone, ball, player.Strokes );
			CuppedBall( To.Everyone, ball );

			Action task = async () =>
			{
				await Task.DelaySeconds(5);

				Course.AdvancedHole();

				// Reset for now
				// player.Strokes = 0;
				ResetBall(ball);
			};
			task.Invoke();
		}

		[ClientRpc]
		protected void CuppedBall( GolfBall ball )
		{
			// Add to UI
			// Sandbox.UI.ChatBox.AddInformation(Player.All, $"{player.Name} scored on hole {hole}!", $"avatar:{player.SteamId}");

			if ( Local.Pawn != ball ) return;

			// nice job bro, hole in one!
			/* if ( strokes == 1 )
				Sound.FromScreen( SoundHoleInOne.Name ).SetVolume( 0.8f );
			else if ( strokes - Course.CurrentHole.Par > 0 )
				Sound.FromScreen( SoundBelowPar.Name ); */

			_ = EndScore.Current.ShowScore( Course.CurrentHole.Number, Course.CurrentHole.Par, 3 );
		}

		protected void ResetBall(GolfBall ball)
		{
			ball.ResetPosition( Course.CurrentHole.SpawnPosition, Course.CurrentHole.SpawnAngles );
		}
	}
}
