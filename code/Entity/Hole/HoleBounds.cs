using System.Linq;
using System.Collections.Generic;
using Sandbox;

namespace Minigolf
{
	[Library("minigolf_hole_bounds")]
	public partial class HoleBounds : BaseTrigger
	{
		[HammerProp("hole_number")]
		public int Hole { get; set; }

		public IEnumerable<PlayerBall> TouchingBalls => touchingBalls;
		private readonly List<PlayerBall> touchingBalls = new();

		public override void StartTouch(Entity other)
		{
			base.StartTouch(other);

			if (other is PlayerBall)
				AddTouchingBall(other as PlayerBall);
		}

		public override void EndTouch(Entity other)
		{
			base.EndTouch(other);

			if (other is not PlayerBall)
				return;

			var ball = other as PlayerBall;

			if (touchingBalls.Contains(ball))
				touchingBalls.Remove(ball);
		}

		protected void AddTouchingBall(PlayerBall ball)
		{
			if (!ball.IsValid())
				return;

			if (!touchingBalls.Contains(ball))
				touchingBalls.Add(ball);
		}
	}
}
