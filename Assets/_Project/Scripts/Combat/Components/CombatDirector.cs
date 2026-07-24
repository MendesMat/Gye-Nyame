using System.Collections.Generic;
using UnityEngine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Combat.Components
{
    public class CombatDirector : MonoBehaviour, IAttackDirector
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private int maxSimultaneousAttacks = 2;
        [SerializeField] private float slotDistance = 2f;

        private readonly HashSet<int> _activeAttackers = new();
        private readonly Dictionary<int, Vector2> _enemyNoises = new();
        private readonly HashSet<GameObject> _registeredAttackers = new();
        private bool _isPlayerDead;

        public Transform GetPlayerTransform() => playerTransform;

        public void RegisterAttacker(GameObject enemy)
        {
            _registeredAttackers.Add(enemy);
        }

        public void UnregisterAttacker(GameObject enemy)
        {
            _registeredAttackers.Remove(enemy);
            ReleaseAttackToken(enemy.GetInstanceID());
        }

        public bool IsTokenAvailableFor(GameObject enemy)
        {
            if (_isPlayerDead) return false;

            int enemyId = enemy.GetInstanceID();

            if (_activeAttackers.Contains(enemyId)) return true;
            if (HasActiveCooldown(enemy)) return false;

            int rank = GetAttackerRankByDistance(enemy);

            return rank < maxSimultaneousAttacks && _activeAttackers.Count < maxSimultaneousAttacks;
        }

        public bool RequestAttackToken(GameObject enemy)
        {
            if (_isPlayerDead) return false;

            int enemyId = enemy.GetInstanceID();

            if (_activeAttackers.Contains(enemyId)) return true;
            if (HasActiveCooldown(enemy)) return false;

            int rank = GetAttackerRankByDistance(enemy);

            if (rank < maxSimultaneousAttacks && _activeAttackers.Count < maxSimultaneousAttacks)
            {
                _activeAttackers.Add(enemyId);
                return true;
            }

            return false;
        }

        private int GetAttackerRankByDistance(GameObject targetEnemy)
        {
            int rank = 0;
            Vector3 playerPosition = playerTransform.position;
            float targetDistance = Vector3.Distance(playerPosition, targetEnemy.transform.position);
            int targetId = targetEnemy.GetInstanceID();

            foreach (GameObject attacker in _registeredAttackers)
            {
                if (attacker == targetEnemy) continue;
                if (attacker == null) continue;
                if (HasActiveCooldown(attacker)) continue;

                float attackerDistance = Vector3.Distance(playerPosition, attacker.transform.position);
                
                if (attackerDistance < targetDistance)
                {
                    rank++;
                    continue;
                }

                if (Mathf.Approximately(attackerDistance, targetDistance) && attacker.GetInstanceID() < targetId)
                {
                    rank++;
                }
            }

            return rank;
        }

        private bool HasActiveCooldown(GameObject enemy)
        {
            var enemyCombat = enemy.GetComponent<IEnemyCombat>();
            return enemyCombat != null && enemyCombat.IsInCooldown;
        }

        public void ReleaseAttackToken(int enemyId) =>  _activeAttackers.Remove(enemyId);

        public Vector3 GetAvailablePositionSlot(GameObject enemy)
        {
            if (_isPlayerDead) return Vector3.zero;
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

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDiedMessage>(OnPlayerDied);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDiedMessage>(OnPlayerDied);
        }

        private void OnPlayerDied(PlayerDiedMessage message)
        {
            _isPlayerDead = true;
            _activeAttackers.Clear();
            _enemyNoises.Clear();
        }
    }
}
