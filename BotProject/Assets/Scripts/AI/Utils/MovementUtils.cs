namespace GameAI.Utils
{
    using UnityEngine;

    using GameUtils;

    public static class MovementUtils
    {
        public static Vector3 ClampVelocity(Vector3 velocity,
                                            float maxSpeed, float slowDownFactor, bool slowWhenNotFacingTarget,
                                            Vector3 forward)
        {
            var currentMaxSpeed = maxSpeed * slowDownFactor;

            if (slowWhenNotFacingTarget && (forward.x != 0 || forward.z != 0))
            {
                float currentSpeed;
                var normalizedVelocity = MathUtils.Normalize(velocity, out currentSpeed);
                float dot = Vector3.Dot(normalizedVelocity, forward);

                float directionSpeedFactor = Mathf.Clamp(dot + 0.707f, 0.2f, 1.0f);
                currentMaxSpeed *= directionSpeedFactor;
                currentSpeed = Mathf.Min(currentSpeed, currentMaxSpeed);

                float angle = Mathf.Acos(Mathf.Clamp(dot, -1, 1));

                angle = Mathf.Min(angle, (20f + 180f * (1 - slowDownFactor * slowDownFactor)) * Mathf.Deg2Rad);

                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);

                sin *= Mathf.Sign(normalizedVelocity.x * forward.y - normalizedVelocity.y * forward.x);

                return new Vector3(forward.x * cos + forward.y * sin, 0, forward.y * cos - forward.x * sin) * currentSpeed;
            }
            else
            {
                return Vector3.ClampMagnitude(velocity, currentMaxSpeed);
            }
        }

        public static Vector3 CalculateAccelerationToReachPoint(Vector3 deltaPosition, Vector3 targetVelocity, Vector3 currentVelocity,
                                                                float forwardsAcceleration, float rotationSpeed, float maxSpeed,
                                                                Vector3 forwardsVector)
        {
            if (forwardsAcceleration <= 0) return Vector3.zero;

            float currentSpeed = currentVelocity.magnitude;
            var sidewaysAcceleration = currentSpeed * rotationSpeed * Mathf.Deg2Rad;
            sidewaysAcceleration = Mathf.Max(sidewaysAcceleration, forwardsAcceleration);

            deltaPosition = MathUtils.ComplexMultiplyConjugate(deltaPosition, forwardsVector);
            targetVelocity = MathUtils.ComplexMultiplyConjugate(targetVelocity, forwardsVector);
            currentVelocity = MathUtils.ComplexMultiplyConjugate(currentVelocity, forwardsVector);
            float ellipseSqrFactorX = 1 / (forwardsAcceleration * forwardsAcceleration);
            float ellipseSqrFactorY = 1 / (sidewaysAcceleration * sidewaysAcceleration);

            if (targetVelocity == Vector3.zero)
            {
                float mn = 0.01f;
                float mx = 10;
                while (mx - mn > 0.01f)
                {
                    var time = (mx + mn) * 0.5f;
                    var a = (6 * deltaPosition - 4 * time * currentVelocity) / (time * time);
                    var q = 6 * (time * currentVelocity - 2 * deltaPosition) / (time * time * time);

                    var nextA = a + q * time;
                    if (a.x * a.x * ellipseSqrFactorX + a.y * a.y * ellipseSqrFactorY > 1.0f || 
                        nextA.x * nextA.x * ellipseSqrFactorX + nextA.y * nextA.y * ellipseSqrFactorY > 1.0f)
                    {
                        mn = time;
                    }                        
                    else
                        mx = time;
                }
                var finalAcceleration = (6 * deltaPosition - 4 * mx * currentVelocity) / (mx * mx);
                {
                    const float Boost = 1;
                    finalAcceleration.y *= 1 + Boost;
                    float ellipseMagnitude = finalAcceleration.x * finalAcceleration.x * ellipseSqrFactorX + finalAcceleration.y * finalAcceleration.y * ellipseSqrFactorY;
                    if (ellipseMagnitude > 1.0f) finalAcceleration /= Mathf.Sqrt(ellipseMagnitude);
                }
                return MathUtils.ComplexMultiply(finalAcceleration, forwardsVector);
            }
            else
            {
                const float TargetVelocityWeight = 0.5f;

                const float TargetVelocityWeightLimit = 1.5f;
                float targetSpeed;
                var normalizedTargetVelocity = MathUtils.Normalize(targetVelocity, out targetSpeed);

                var distance = deltaPosition.magnitude;
                var targetPoint = deltaPosition - normalizedTargetVelocity * System.Math.Min(TargetVelocityWeight * distance * targetSpeed / (currentSpeed + targetSpeed), maxSpeed * TargetVelocityWeightLimit);

                const float TimeToReachDesiredVelocity = 0.1f;

                var finalAcceleration = (targetPoint.normalized * maxSpeed - currentVelocity) * (1f / TimeToReachDesiredVelocity);

                float ellipseMagnitude = finalAcceleration.x * finalAcceleration.x * ellipseSqrFactorX + finalAcceleration.y * finalAcceleration.y * ellipseSqrFactorY;
                if (ellipseMagnitude > 1.0f) finalAcceleration /= Mathf.Sqrt(ellipseMagnitude);

                return MathUtils.ComplexMultiply(finalAcceleration, forwardsVector);
            }
        }
    }
}