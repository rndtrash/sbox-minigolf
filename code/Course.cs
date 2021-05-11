using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Minigolf
{
    partial class Course : NetworkClass
    {
        public string Name { get; set; } = "Default";
        public string Description { get; set; } = "Default Description";
        public string CameraPath { get; set; } // Placeholder
        public Dictionary<int, HoleInfo> Holes { get; set; } = new Dictionary<int, HoleInfo>();

        protected int currentHole = 1;
        public HoleInfo CurrentHole {
            get => Holes.GetValueOrDefault(currentHole);
            set => currentHole = value.Number;
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

                Holes[hole.Number] = new HoleInfo()
                {
                    Number = hole.Number,
                    Name = hole.Name,
                    Par = hole.Par,
                    SpawnPosition = hole.WorldPos,
                    GoalPosition = goal.WorldPos,
                    Bounds = Entity.All.OfType<HoleBounds>().Where(x => x.Hole == hole.Number).ToList()
                };
            }
        }

		// TODO: SHIT CODE
        public void AdvancedHole()
        {
			// Next hole is
			var nextHoleKey = Holes.Where( x => x.Key > currentHole ).OrderBy( x => x.Key ).FirstOrDefault();

			// No more holes, just loop around for now.
			if ( nextHoleKey.Value == null )
				currentHole = 1;

			// Advanced to next hole, TODO: ClientRpc
			currentHole = nextHoleKey.Key;
        }

        public override bool NetWrite(NetWrite write)
        {
            write.WriteUtf8(Name);
            write.WriteUtf8(Description);

            write.Write(Holes.Count);
            foreach (var hole in Holes)
            {
                write.Write(hole.Key);
                write.Write(hole.Value);
            }

            write.Write(currentHole);

            return true;
        }

        public override bool NetRead(NetRead read)
        {
            Name = read.ReadUtf8();
            Description = read.ReadUtf8();

            var count = read.Read<int>();

            Holes = new();
            for (int i = 0; i < count; i++)
            {
                var k = read.Read<int>();
                Holes[k] = read.ReadClass<HoleInfo>(new HoleInfo());
            }

            currentHole = read.Read<int>();

            return true;
        }
    }

    class HoleInfo : NetworkClass
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public int Par { get; set; }
        public Vector3 SpawnPosition { get; set; } = Vector3.Zero;
        public Vector3 GoalPosition { get; set; } = Vector3.Zero;
        public List<HoleBounds> Bounds { get; set; } = new List<HoleBounds>();

        public bool InBounds(Entity other)
        {
            return Bounds.Where(x => x.TouchingBalls.Contains(other)).Any();
        }

        #region Networking
        // Run our own NetRead/Write, it gets really confused with List<HoleBounds>.

        public override bool NetWrite(NetWrite write)
        {
            write.Write(Number);
            write.WriteUtf8(Name);
            write.Write(Par);
            write.Write(SpawnPosition);
            write.Write(GoalPosition);

            return true;
        }


        public override bool NetRead(NetRead read)
        {
            Number = read.Read<int>();
            Name = read.ReadUtf8();
            Par = read.Read<int>();
            SpawnPosition = read.Read<Vector3>();
            GoalPosition = read.Read<Vector3>();
            
            return true;
        }
        #endregion
    }

}
