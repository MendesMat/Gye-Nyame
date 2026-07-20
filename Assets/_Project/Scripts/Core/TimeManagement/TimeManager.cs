using System.Collections;
using UnityEngine;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Core.TimeManagement
{
    public class TimeManager : MonoBehaviour
    {
        private Coroutine _timeCoroutine;

        private void OnEnable()
        {
            EventBus.Subscribe<EntityDamagedMessage>(HandleEntityDamaged);
            EventBus.Subscribe<SlowMotionRequestMessage>(HandleSlowMotionRequest);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EntityDamagedMessage>(HandleEntityDamaged);
            EventBus.Unsubscribe<SlowMotionRequestMessage>(HandleSlowMotionRequest);
        }

        private void HandleEntityDamaged(EntityDamagedMessage message)
        {
            float hitStopTime = message.Damage.HitStopTime;
            if (hitStopTime <= 0f) return;
            
            if (_timeCoroutine != null) StopCoroutine(_timeCoroutine);
            _timeCoroutine = StartCoroutine(TimeRoutine(hitStopTime, 0f));
        }

        private void HandleSlowMotionRequest(SlowMotionRequestMessage message)
        {
            if (message.Duration <= 0f) return;
            
            if (_timeCoroutine != null) StopCoroutine(_timeCoroutine);
            _timeCoroutine = StartCoroutine(TimeRoutine(message.Duration, message.TimeScale));
        }

        private IEnumerator TimeRoutine(float duration, float targetTimeScale)
        {
            Time.timeScale = targetTimeScale;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }
    }
}
