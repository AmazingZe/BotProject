namespace GameRuntime
{
    using UnityEngine;
    using UnityEditor;

    public class MoveConfigure : Configure
    {
        #region Properties
        public bool UpdatePosition = true;
        public bool UpdateRotation = true;
        public bool CanMove = true;
        public bool IsStopped = false;
        public float RotationSpeed = 60;
        public float MaxAcceleration = 1;
        public float MaxSpeed = 3;
        public float LookAheadDis = 2f;
        public float EndReachedDis = 0.2f;
        public float SlowDownDis = 0.6f;
        public bool SlowWhenNotFacingTarget = false;
        #endregion

        public override string TitleName
        {
            get { return "MoveConfigure"; }
        }

        protected override void OnDraw()
        {
            UpdatePosition = EditorUtils.BoolFieldWithLabel("UpdatePosition", UpdatePosition);
            UpdateRotation = EditorUtils.BoolFieldWithLabel("UpdateRotation", UpdateRotation);
            CanMove = EditorUtils.BoolFieldWithLabel("CanMove", CanMove);
            IsStopped = EditorUtils.BoolFieldWithLabel("IsStopped", IsStopped);
            SlowWhenNotFacingTarget = EditorUtils.BoolFieldWithLabel("SlowWhenNotFacingTarget", SlowWhenNotFacingTarget);

            RotationSpeed = EditorUtils.FloatFieldWithLabel("RotationSpeed", RotationSpeed);
            MaxAcceleration = EditorUtils.FloatFieldWithLabel("MaxAcceleration", MaxAcceleration);
            MaxSpeed = EditorUtils.FloatFieldWithLabel("MaxSpeed", MaxSpeed);
            LookAheadDis = EditorUtils.FloatFieldWithLabel("LookAheadDis", LookAheadDis);
            EndReachedDis = EditorUtils.FloatFieldWithLabel("EndReachedDis", EndReachedDis);
            SlowDownDis = EditorUtils.FloatFieldWithLabel("SlowDownDis", SlowDownDis);
        }
    }
}