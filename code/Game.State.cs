using Sandbox;

namespace Minigolf
{
	partial class GolfGame
	{
		public enum GameState
		{
			WaitingForPlayers,
			Playing,
			EndOfGame
		}

		// [Net] public bool WaitingToStart { get; set; } = true;
		// [Net] public float StartTime { get; set; }
	}
}
