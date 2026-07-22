using UnityEngine;

namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IAttackDirector
    {
        Transform GetPlayerTransform();
        bool RequestAttackToken(GameObject enemy);
        void ReleaseAttackToken(int enemyId);
        Vector3 GetAvailablePositionSlot(GameObject enemy);
        bool IsInAttackRange(GameObject enemy);
    }
}
