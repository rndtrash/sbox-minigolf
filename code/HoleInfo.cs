using Sandbox;
using System.Linq;

namespace Minigolf
{
    class HoleInfo
    {
        public int Number { get; set; }
        public string Name { get; set;  }
        public int Par { get; set; }

        public HoleInfo(int number, string name, int par)
        {
            Number = number;
            Name = name;
            Par = par;  
        }

        static public bool InBounds(int hole, Entity ent)
        {
            // could be more performant to store refs to HoleBounds per hole, micro-optimization
            return Entity.All.OfType<HoleBounds>().Where(x => x.Hole == hole && x.TouchingBalls.Contains(ent)).Count() > 0;
        }
    }
}
