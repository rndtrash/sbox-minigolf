using System.Linq;
using System.Collections.Generic;
using Sandbox;
using Sandbox.Internal;

namespace Minigolf
{
	[Library("minigolf_hole_bounds", Description = "Minigolf Bounds of a Hole" )]
	[Hammer.Solid]
	[Hammer.AutoApplyMaterial]
	public partial class HoleBounds : ModelEntity
	{
		/// <summary>
		/// Which hole this hole is on.
		/// </summary>
		[Property( Name = "hole_number", Title = "Hole Number" )]
		public int Hole { get; set; } = 1;

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
