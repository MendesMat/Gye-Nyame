using UnityEngine;
using GyeNyame.Combat.Data;

namespace GyeNyame.Enemy.Combat
{
    public class EnemyCombat : MonoBehaviour
    {
        [Header("Combat Data")]
        [SerializeField] private AttackDataSO[] attacks;
        
        public void PerformAttack(int attackIndex)
        {
            if (attacks == null) return;
            if (attackIndex < 0) return;
            if (attackIndex >= attacks.Length) return;
        }
    }
}
