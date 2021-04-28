using Sandbox;

namespace Minigolf
{
	/// <summary>
	/// Empty Player Animator since we don't have anything to animate
	/// </summary>
	partial class EmptyPlayerAnimator : PlayerAnimator
	{
		public override void Tick() { }
	}

	partial class EmptyPlayerController : PlayerController
	{
        public override BBox GetHull() => new( -10, 10 );
		public override void Tick() { }
    }

	/// <summary>
	/// All the junk we don't want
	/// </summary>
	partial class GolfPlayer
	{
		private void SetupControllerAndAnimator()
        {
			Controller = new EmptyPlayerController();
			Animator = new EmptyPlayerAnimator();
        }

		[NetPredicted]
		private PlayerController Controller { get; set; }

		[NetPredicted]
		private PlayerAnimator Animator { get; set; }

		public override PlayerController GetActiveController() => Controller;
		public override PlayerAnimator GetActiveAnimator() => Animator;
	}
}
