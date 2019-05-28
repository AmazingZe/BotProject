namespace GameAI.Component
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameRuntime;
    using GameAI.Utils;

    public class MoveContoller : IComponent
    {
        #region Move_Properties
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

        public bool ReachedEndOfPath;

        private PathInterpolator interpolator;

        private Vector3 Velocity;
        private Vector3 m_SimulatedPosition;
        private Quaternion m_SimulatedRotation;
        private Transform transform;

        private float m_LastDeltatime;
        private Vector3 m_LastDeltaPosition;
        #endregion

        #region Public_Properties
        public Vector3 Position
        {
            get { return UpdatePosition ? transform.position : m_SimulatedPosition; }
        }
        public Vector3 SteerPosition
        {
            get { return interpolator.Valid ? interpolator.CurPosition : Position; }
        }
        public Quaternion Rotation
        {
            get { return UpdateRotation ? transform.rotation : m_SimulatedRotation; }
        }
        #endregion

        private MoveContoller() { }
        public static MoveContoller Create()
        {
            MoveContoller retMe = new MoveContoller();
            retMe.OnInit();
            return retMe;
        }

        #region IComponent
        public BotBehaviour Owner { get; set; }
        public int Priority { get; set; }
        public MsgCenter Msgcenter { get; set; }

        public void OnInit()
        {
            interpolator = new PathInterpolator();
            List<Vector3> path = new List<Vector3>() { new Vector3(2, 0, 0),
                                                       new Vector3(3, 0, 1),
                                                       new Vector3(3, 0, 3) };
            interpolator.SetPath(path);
        }
        public void OnRelease()
        {

        }
        public void OnUpdate()
        {
            if (!CanMove) return;

            Vector3 nextPosition;
            Quaternion nextRotation;
            MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
            FinalizeMovement(nextPosition, nextRotation);
        }
        public void SetOwner(BotBehaviour owner)
        {
            Owner = owner;
            transform = owner.transform;
        }
        public void OnNotify(int msgID)
        {

        }
        #endregion

        private void MovementUpdate(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
        {
            m_LastDeltatime = deltaTime;

            float currentAcceleration = MaxAcceleration;

            if (UpdatePosition) m_SimulatedPosition = transform.position;
            if (UpdateRotation) m_SimulatedRotation = transform.rotation;

            var currentPosition = m_SimulatedPosition;
            interpolator.MoveToCircleIntersection(currentPosition, LookAheadDis);
            var dir = (SteerPosition - currentPosition);

            float distanceToEnd = dir.magnitude + Mathf.Max(0, interpolator.RemainingDistance);

            var prevTargetReached = ReachedEndOfPath;
            ReachedEndOfPath = distanceToEnd <= EndReachedDis && interpolator.Valid;
            if (!prevTargetReached && ReachedEndOfPath)
                OnTargetReached();

            float slowDown;

            var forward = m_SimulatedRotation * Vector3.forward;
            if (interpolator.Valid && !IsStopped)
            {
                slowDown = distanceToEnd < SlowDownDis ? Mathf.Sqrt(distanceToEnd / SlowDownDis) : 1;

                //Todo: Get end of path mode
                if (ReachedEndOfPath)
                    Velocity -= Vector3.ClampMagnitude(Velocity, currentAcceleration * deltaTime);
                else
                    Velocity += MovementUtils.CalculateAccelerationToReachPoint(dir, dir.normalized * MaxSpeed, Velocity,
                                                                                currentAcceleration, RotationSpeed, MaxSpeed, forward) * deltaTime;
            }
            else
            {
                slowDown = 1;
                Velocity -= Vector3.ClampMagnitude(Velocity, currentAcceleration * deltaTime);
            }

            Velocity = MovementUtils.ClampVelocity(Velocity, MaxSpeed, slowDown, SlowWhenNotFacingTarget, forward);

            //Todo: RVO Controller

            var delta = m_LastDeltaPosition = CalculateDeltaToMoveThisFrame(distanceToEnd, deltaTime);
            nextPosition = currentPosition + delta;

            CalculateNextRotation(slowDown, out nextRotation);
        }
        private void CalculateNextRotation(float slowDown, out Quaternion nextRotation)
        {
            if (m_LastDeltatime > 0.01f)
            {
                //Todo: RVO
                Vector3 desiredRotationDirection = Velocity;
                var currentRotationSpeed = RotationSpeed * Mathf.Max(0, (slowDown - 0.3f) / 0.7f);
                nextRotation = SimulateRotationTowards(desiredRotationDirection, currentRotationSpeed * m_LastDeltatime);
            }
            else
            {
                nextRotation = Rotation;
            }
        }
        private Quaternion SimulateRotationTowards(Vector3 direction, float maxDegrees)
        {
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.zero);

                return Quaternion.RotateTowards(m_SimulatedRotation, targetRotation, maxDegrees);
            }

            return m_SimulatedRotation;
        }
        private Vector3 CalculateDeltaToMoveThisFrame(float distanceToEndOfPath, float deltaTime)
        {
            //Todo: RVO

            return Vector3.ClampMagnitude(Velocity * deltaTime, distanceToEndOfPath);
        }

        private void FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation)
        {
            FinalizeRotation(nextRotation);
            FinalizePosition(nextPosition);
        }
        private void FinalizePosition(Vector3 nextPosition)
        {
            m_SimulatedPosition = nextPosition;

            if (UpdatePosition)
                transform.position = m_SimulatedPosition;
        }
        private void FinalizeRotation(Quaternion nextRotation)
        {
            m_SimulatedRotation = nextRotation;
            if (UpdateRotation)
                transform.rotation = nextRotation;
        }

        private void OnTargetReached()
        {

        }
    }
}