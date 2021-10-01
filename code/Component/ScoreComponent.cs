using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Minigolf
{
	public partial class ScoreComponent : EntityComponent, INetworkSerializer
	{
		public int Score
		{
			get
			{
				if ( Scores.ContainsKey( Game.Current.Course.CurrentHole.Number ) )
					return Scores[Game.Current.Course.CurrentHole.Number];
				return 0;
			}
			set
			{
				Scores[Game.Current.Course.CurrentHole.Number] = value;
				WriteNetworkData();
			}
		}
		public Dictionary<int, int> Scores { get; set; } = new();
		public int Total => Scores.Values.Sum();

		void INetworkSerializer.Write( NetWrite write )
		{
			write.Write( Scores.Count );
			foreach ( var score in Scores )
			{
				write.Write( score.Key );
				write.Write( score.Value );
			}

			Log.Info( $"{this} Wrote {Scores.Count}" );
		}

		void INetworkSerializer.Read( NetRead read )
		{
			Scores = new();

			var count = read.Read<int>();
			for ( int i = 0; i < count; i++ )
			{
				var key = read.Read<int>();
				var value = read.Read<int>();

				Scores[key] = value;
			}

			Log.Info( $"{this} Read {count}" );
		}
	}
}
