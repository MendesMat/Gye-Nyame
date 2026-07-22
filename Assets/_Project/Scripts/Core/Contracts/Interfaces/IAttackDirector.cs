using UnityEngine;

namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IAttackDirector
    {
        Transform GetPlayerTransform();
        void RegisterAttacker(GameObject enemy);
        void UnregisterAttacker(GameObject enemy);
        bool RequestAttackToken(GameObject enemy);
        void ReleaseAttackToken(int enemyId);
        Vector3 GetAvailablePositionSlot(GameObject enemy);
        bool IsInAttackRange(GameObject enemy);
    }
}
