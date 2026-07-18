using UnityEngine;

namespace GyeNyame.Combat.Data
{
    [CreateAssetMenu(fileName = "NewAttackData", menuName = "GyeNyame/Combat/Attack Data")]
    public class AttackDataSO : ScriptableObject
    {
        [Header("Combat Stats")]
        [SerializeField] private float damage = 10f;
        
        [Header("Input")]
        [SerializeField] private float bufferTime = 0.2f;
        
        [Header("Repulsion (Kinematics)")]
        [SerializeField] private float knockbackForce = 15f;
        [SerializeField] private float knockupForce = 0f;

        [Header("Effects")]
        [SerializeField] private float hitStopTime = 0.1f;
        [SerializeField] private float screenShakeMultiplier = 0f;

        public float Damage => damage;
        public float BufferTime => bufferTime;
        public float KnockbackForce => knockbackForce;
        public float KnockupForce => knockupForce;
        public float HitStopTime => hitStopTime;
        public float ScreenShakeMultiplier => screenShakeMultiplier;
    }
}
