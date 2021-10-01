using Sandbox;
using System.Linq;

namespace Minigolf
{
	public static partial class ClientExtensions
	{
		public static Entity GetEntity( this Client self ) => Entity.FindByIndex( self.NetworkIdent );
		public static SteamID SteamID( this Client self ) => new( self.SteamId );
		public static bool IsBot( this Client self ) => self.SteamID().AccountType == SteamIDAccountType.AnonGameServer;
		public static bool IsHost( this Client self ) => Global.IsListenServer && self.NetworkIdent == 1;
	}
}
