using Sandbox;
using System.Collections.Generic;
using System.IO;

namespace Minigolf
{
	public partial class Course
	{
		/// <summary>
		/// Sends serialized course info to the specified Client.
		/// </summary>
		/// <param name="to"></param>
		public void SendCourseInfo( To to )
		{
			using ( var stream = new MemoryStream() )
			{
				using ( var writer = new BinaryWriter( stream ) )
				{
					writer.Write( Name );
					writer.Write( Description );
					writer.Write( _currentHole );

					writer.Write( Holes.Count );
					foreach ( var holePair in Holes )
					{
						var hole = holePair.Value;
						writer.Write( holePair.Key );
						writer.Write( hole.Name );
						writer.Write( hole.Par );
					}	
				}

				clientSetupCourse( to, stream.ToArray() );
			}
		}

		[ClientRpc]
		static public void clientSetupCourse( byte[] data )
		{
			if ( GolfGame.Instance == null )
			{
				Log.Error( "Tried to setup course before Game constructed." );
				return;
			}

			GolfGame.Instance.Course = new Course();
			var course = GolfGame.Instance.Course;

			// Read in our data
			using ( var reader = new BinaryReader( new MemoryStream( data ) ) )
			{
				course.Name = reader.ReadString();
				course.Description = reader.ReadString();
				course._currentHole = reader.ReadInt32();

				var count = reader.ReadInt32();
				course.Holes = new Dictionary<int, HoleInfo>( count );

				for ( int i = 0; i < count; i++ )
				{
					var num = reader.ReadInt32();

					HoleInfo hole = new HoleInfo();
					hole.Number = num;
					hole.Name = reader.ReadString();
					hole.Par = reader.ReadInt32();

					course.Holes[num] = hole;
				}
			}
		}
	}
}
