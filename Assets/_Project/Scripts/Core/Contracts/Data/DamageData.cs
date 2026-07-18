using UnityEngine;

namespace GyeNyame.Core.Contracts.Data
{
    public struct DamageData
    {
        public float Amount { get; }
        public Vector3 SourcePosition { get; }
        public float KnockbackForce { get; }
        public float KnockupForce { get; }
        public float HitStopTime { get; }

        public DamageData(float amount, Vector3 sourcePosition, float knockbackForce, float knockupForce, float hitStopTime)
        {
            Amount = amount;
            SourcePosition = sourcePosition;
            KnockbackForce = knockbackForce;
            KnockupForce = knockupForce;
            HitStopTime = hitStopTime;
        }
    }
}
