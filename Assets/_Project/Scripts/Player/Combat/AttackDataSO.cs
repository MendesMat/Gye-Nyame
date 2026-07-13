using UnityEngine;

namespace GyeNyame.Player.Combat
{
    [CreateAssetMenu(fileName = "NewAttackData", menuName = "GyeNyame/Attack Data")]
    public class AttackDataSO : ScriptableObject
    {
        [Header("Combat Stats")]
        [SerializeField] private float damage = 10f;
        
        [Header("Input")]
        [SerializeField] private float bufferTime = 0.2f;
        
        [Header("Effects")]
        [SerializeField] private float hitStopTime = 0.1f;
        [SerializeField] private float screenShakeMultiplier = 0f;

        public float Damage => damage;
        public float BufferTime => bufferTime;
        public float HitStopTime => hitStopTime;
        public float ScreenShakeMultiplier => screenShakeMultiplier;
    }
}
