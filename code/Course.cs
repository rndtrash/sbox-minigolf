using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Minigolf
{
	// TODO: This is an Entity for no reason other then NetworkComponent struggles with Lists
    partial class Course : Entity
	{
		[Net]
        public string Name { get; set; } = "Default";
		[Net]
        public string Description { get; set; } = "Default Description";
		[Net]
		public List<HoleInfo> Holes { get; set; }
		[Net]
		private int _currentHole { get; set; } = 1;

		// todo: make this more sane.. maybe even nullable?
        public HoleInfo CurrentHole {
			get
			{
				// Bit shit I'm iterating here, but who cares
				return Holes.Where( ( x ) => x.Number == _currentHole ).FirstOrDefault();
			}
        }

		public override void Spawn()
		{
			base.Spawn();
			Transmit = TransmitType.Always;
		}

		/// <summary>
		/// Load the course info from the current map.
		/// </summary>
		public void LoadFromMap()
        {
            Host.AssertServer();

            Holes.Clear();
            foreach (var hole in Entity.All.OfType<HoleSpawn>())
            {
                var goal = Entity.All.OfType<HoleGoal>().Where(x => x.Hole == hole.Number).First();

                if (goal == null)
                {
                    Log.Error($"No ball goal found for [Hole {hole.Number}]");
                    continue;
                }

				Log.Info( $"Hole ({hole.Number}) - {hole.Name}" );

				// todo: sort this list

				Holes.Add(new HoleInfo()
                {
                    Number = hole.Number,
                    // Name = hole.Name,
                    Par = hole.Par,
                    SpawnPosition = hole.Position,
                    SpawnAngles = hole.WorldAng,
					// GoalPosition = goal.Position,
                    // Bounds = Entity.All.OfType<HoleBounds>().Where(x => x.Hole == hole.Number).ToList()
                });
            }
        }

		// TODO: SHIT CODE
        public void AdvancedHole()
        {
			// Next hole is
			// var nextHoleKey = Holes.Where( x => x.Key > currentHole ).OrderBy( x => x.Key ).First();

			// No more holes, just loop around for now.
			// if ( nextHoleKey.Value == null )
			// 	currentHole = 1;

			// Advanced to next hole, TODO: ClientRpc
			// currentHole = nextHoleKey.Key;
        }
    }

    public struct HoleInfo
    {
		public int Number;
		// fuck me why can't i network a string
		// public string Name;
		public int Par;
		public Vector3 SpawnPosition;
		public Angles SpawnAngles;
    }
}
