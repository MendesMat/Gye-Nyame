using System.Collections.Generic;
using UnityEngine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Combat.Components
{
    public class CombatDirector : MonoBehaviour, IAttackDirector
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private int maxSimultaneousAttacks = 2;
        [SerializeField] private float slotDistance = 2f;

        private readonly HashSet<int> _activeAttackers = new();
        private readonly Dictionary<int, Vector2> _enemyNoises = new();

        public Transform GetPlayerTransform() => playerTransform;

        public bool RequestAttackToken(GameObject enemy)
        {
            int enemyId = enemy.GetInstanceID();

            if (HasActiveCooldown(enemy)) return false;
            if (_activeAttackers.Contains(enemyId)) return true;
            if (_activeAttackers.Count >= maxSimultaneousAttacks) return false;

            _activeAttackers.Add(enemyId);
            return true;
        }

        private bool HasActiveCooldown(GameObject enemy)
        {
            var enemyCombat = enemy.GetComponent<IEnemyCombat>();
            return enemyCombat != null && enemyCombat.IsInCooldown;
        }

        public void ReleaseAttackToken(int enemyId) =>  _activeAttackers.Remove(enemyId);

        public Vector3 GetAvailablePositionSlot(GameObject enemy)
        {
            if (playerTransform == null || enemy == null) return Vector3.zero;

            int enemyId = enemy.GetInstanceID();
            
            if (!_enemyNoises.TryGetValue(enemyId, out Vector2 noise))
            {
                noise = new Vector2(Random.Range(-0.3f, 0.3f), 0f);
                _enemyNoises[enemyId] = noise;
            }

            float side = Mathf.Sign(enemy.transform.position.x - playerTransform.position.x);
            
            Vector3 baseOffset = new Vector3(side * slotDistance, 0, 0);
            Vector3 noiseOffset = new Vector3(noise.x, 0f, 0f);

            return playerTransform.position + baseOffset + noiseOffset;
        }

        public bool IsInAttackRange(GameObject enemy)
        {
            if (playerTransform == null || enemy == null) return false;

            float xDiff = Mathf.Abs(enemy.transform.position.x - playerTransform.position.x);
            float zDiff = Mathf.Abs(enemy.transform.position.z - playerTransform.position.z);

            return xDiff <= (slotDistance + 0.5f) && zDiff <= 0.1f;
        }

        public void ReleasePositionSlot(int enemyId) => _enemyNoises.Remove(enemyId);
    }
}
