using Sandbox;
using System;

namespace Minigolf
{
    public class StaticCamera : BaseCamera
    {
        public StaticCamera()
        {
            Pos = new Vector3(-1910, 744, 616);
            Rot = Rotation.From(19, -60, 0);
            FieldOfView = 90;
        }

        public override void Update()
        {

        }
    }
}