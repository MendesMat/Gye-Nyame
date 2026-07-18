using System.Collections;
using UnityEngine;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Core.TimeManagement
{
    public class HitStopManager : MonoBehaviour
    {
        private Coroutine _hitStopCoroutine;

        private void OnEnable()
        {
            EventBus.Subscribe<EntityDamagedMessage>(HandleEntityDamaged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EntityDamagedMessage>(HandleEntityDamaged);
        }

        private void HandleEntityDamaged(EntityDamagedMessage message)
        {
            float hitStopTime = message.Damage.HitStopTime;
            
            if (hitStopTime <= 0f) return;
            if (_hitStopCoroutine != null) StopCoroutine(_hitStopCoroutine);

            _hitStopCoroutine = StartCoroutine(HitStopRoutine(hitStopTime));
        }

        private IEnumerator HitStopRoutine(float duration)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }
    }
}
