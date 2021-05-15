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

		public IEnumerable<GolfBall> TouchingBalls => touchingBalls;
		private readonly List<GolfBall> touchingBalls = new();

		public override void StartTouch(Entity other)
		{
			base.StartTouch(other);

			if (other is GolfBall ball)
				AddTouchingBall( ball );
		}

		public override void EndTouch(Entity other)
		{
			base.EndTouch(other);

			if ( other is not GolfBall )
				return;

			var ball = other as GolfBall;

			if (touchingBalls.Contains(ball))
				touchingBalls.Remove(ball);
		}

		protected void AddTouchingBall( GolfBall ball )
		{
			if (!ball.IsValid())
				return;

			if (!touchingBalls.Contains(ball))
				touchingBalls.Add(ball);
		}
	}
}
