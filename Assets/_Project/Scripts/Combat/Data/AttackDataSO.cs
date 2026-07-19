using UnityEngine;
using GyeNyame.Core.StateMachine;

namespace GyeNyame.Combat.Data
{
    [CreateAssetMenu(fileName = "NewAttackData", menuName = "GyeNyame/Combat/Attack Data")]
    public class AttackDataSO : ScriptableObject
    {
        [Header("Combat Stats")]
        [SerializeField] private float damage = 10f;
        
        [Header("Combo Tree")]
        [SerializeField] private AttackDataSO nextLightCombo;
        [SerializeField] private AttackDataSO nextHeavyCombo;

        [Header("State Info")]
        [SerializeField] private EntityStateCategory animationCategory;

        [Header("Input & Timing")]
        [SerializeField] private float bufferTime = 0.2f;
        [SerializeField, Tooltip("Deixe em 0 se este ataque finaliza a sequência e não aceita combo.")] 
        private float comboWindowTime = 0.2f;
        [SerializeField, Tooltip("Tempo travado ANTES de poder iniciar um novo combo após esse ataque terminar.")]
        private float cooldownTime = 0.0f;

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
        public float ComboWindowTime => comboWindowTime;
        public float CooldownTime => cooldownTime;
        public AttackDataSO NextLightCombo => nextLightCombo;
        public AttackDataSO NextHeavyCombo => nextHeavyCombo;
        public EntityStateCategory AnimationCategory => animationCategory;
    }
}
