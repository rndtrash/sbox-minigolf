using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Minigolf
{
	/// <summary>
	/// Custom network synced class
	/// </summary>
    public partial class Course
	{
        public string Name { get; set; } = "Default";
        public string Description { get; set; } = "Default Description";
		public Dictionary<int, HoleInfo> Holes { get; set; }

		private int _currentHole { get; set; } = 1;
		public HoleInfo CurrentHole => Holes[_currentHole];

		/// <summary>
		/// Load the course info from the current map.
		/// </summary>
		public void LoadFromMap()
        {
            Host.AssertServer();

			// Todo: Load Name / Description in

			Holes = new Dictionary<int, HoleInfo>();

            foreach (var hole in Entity.All.OfType<BallSpawnpoint>().OrderBy(ent => ent.HoleNumber))
            {
                var goal = Entity.All.OfType<HoleGoal>().Where(x => x.HoleNumber == hole.HoleNumber).First();

                if (goal == null)
                {
                    Log.Error($"No ball goal found for [Hole {hole.HoleNumber}]");
                    continue;
                }

				Holes.Add(hole.HoleNumber, new HoleInfo()
                {
                    Number = hole.HoleNumber,
                    Name = hole.HoleName,
                    Par = hole.HolePar,
                    SpawnPosition = hole.Position,
                    SpawnAngles = hole.WorldAng,
                });
            }
        }

		// TODO: SHIT CODE
        public void NextHole()
        {
			var matchedHoles = Holes.Where( x => x.Key > _currentHole )
				.OrderBy( x => x.Key );

			// No more holes to advance to? Return early.
			// This should be checked before calling this function.
			if ( !matchedHoles.Any() )
				return;

			var nextHole = matchedHoles.First();

			_currentHole = nextHole.Key;

			// Announce to all clients that the course has advanced.
			clientNextHole( To.Everyone, _currentHole );

			// Run an event so we can pick this up anywhere in the code base.
			Event.Run( "minigolf.advanced_hole", _currentHole );
        }

		[ClientRpc]
		public static void clientNextHole( int newHole )
		{
			var course = GolfGame.Instance.Course;
			if ( course == null )
			{
				Log.Error( "Tried to advance hole with no Course instance." );
				return;
			}

			course._currentHole = newHole;
			Event.Run( "minigolf.advanced_hole", course._currentHole );
		}

		[ServerCmd( "minigolf_debug_print_sv" )]
		static void PrintCourse()
		{
			var game = Game.Current as GolfGame;
			Log.Info( $"Coruse: {game.Course}, {game.Course.CurrentHole}, {game.Course.Holes}" );
			Log.Info( $"{game.Course.Holes.Count} holes" );
			foreach ( var hole in game.Course.Holes.Values )
			{
				Log.Info( $"\t[{hole.Number}] name = {hole.Name} par = {hole.Par}" );
			}
		}

		[ClientCmd( "minigolf_debug_print_cl" )]
		static void PrintCourseCl()
		{
			PrintCourse();
		}
	}

	public struct HoleInfo
    {
		public int Number;
		public string Name;
		public int Par;
		public Vector3 SpawnPosition;
		public Angles SpawnAngles;
    }
}
