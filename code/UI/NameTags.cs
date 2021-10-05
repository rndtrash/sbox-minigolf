using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Minigolf
{
	public class BaseNameTag : WorldPanel
	{
		public Label NameLabel;
		public Image Avatar;

		Entity entity;

		public BaseNameTag( Entity entity )
		{
			StyleSheet.Load( "/ui/NameTags.scss" );

			this.entity = entity;

			var client = entity.Client;

			NameLabel = Add.Label( $"{client.Name}" );
			Avatar = Add.Image( $"avatar:{client.SteamId}" );
		}

		public virtual void UpdateFromPlayer( Entity entity )
		{
			Position = entity.Position + Game.Current.BallCamera.AnglesTowardsPlayer.ToRotation().Left * 25;
			Rotation = Game.Current.BallCamera.AnglesTowardsPlayer.ToRotation();
		}
	}

	public class NameTags : Entity
	{
		Dictionary<Entity, BaseNameTag> ActiveTags = new Dictionary<Entity, BaseNameTag>();

		public NameTags()
		{
			Transmit = TransmitType.Always;
		}

		[Event.Frame]
		public void Tick()
		{
			var deleteList = new List<Entity>();
			deleteList.AddRange(ActiveTags.Keys);

			foreach (var entity in Entity.All.OfType<Ball>().OrderBy(x => Vector3.DistanceBetween(x.EyePos, CurrentView.Position ) ))
			{
				if (UpdateNameTag(entity))
					deleteList.Remove(entity);
			}

			foreach (var player in deleteList)
			{
				ActiveTags[player].Delete();
				ActiveTags.Remove(player);
			}
		}

		public bool UpdateNameTag( Entity entity )
		{
			// If there's a ball without an owner remove it
			if ( !entity.Client.IsValid() )
				return false;

			if (!ActiveTags.TryGetValue(entity, out var tag))
			{
				tag = new BaseNameTag( entity );
				ActiveTags[entity] = tag;
			}

			tag.UpdateFromPlayer(entity);

			return true;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			foreach (var at in ActiveTags)
			{
				at.Value.Delete();
			}
		}
	}
}
