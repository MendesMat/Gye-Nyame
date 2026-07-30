using UnityEngine;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Core.GameFlow
{
    public class LevelEnemyTracker : MonoBehaviour
    {
        private int _totalEnemies;

        private void Start() => CountInitialEnemies();

        private void OnEnable()
        {
            EventBus.Subscribe<EntityDeadMessage>(OnEntityDead);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EntityDeadMessage>(OnEntityDead);
        }

        private void CountInitialEnemies()
        {
            MonoBehaviour[] allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            _totalEnemies = 0;
            foreach (MonoBehaviour behaviour in allBehaviours)
            {
                if (behaviour is IEnemyCombat) _totalEnemies++;
            }
        }

        private void OnEntityDead(EntityDeadMessage message)
        {
            if (!IsEnemy(message.Target)) return;

            DecrementEnemyCount();
        }

        private bool IsEnemy(GameObject target)
        {
            if (target == null) return false;

            return target.TryGetComponent(out IEnemyCombat _);
        }

        private void DecrementEnemyCount()
        {
            if (_totalEnemies <= 0) return;

            _totalEnemies--;

            if (_totalEnemies == 0) NotifyAllEnemiesDefeated();
        }

        private void NotifyAllEnemiesDefeated() => EventBus.Publish(new AllEnemiesDefeatedMessage());
    }
}
