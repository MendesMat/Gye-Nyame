using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Core.GameFlow
{
    public class GameResetHandler : MonoBehaviour
    {
        [SerializeField] private float resetDelay = 3f;

        private void OnEnable()
        {
            EventBus.Subscribe<GameOverMessage>(OnGameOver);
            EventBus.Subscribe<AllEnemiesDefeatedMessage>(OnAllEnemiesDefeated);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameOverMessage>(OnGameOver);
            EventBus.Unsubscribe<AllEnemiesDefeatedMessage>(OnAllEnemiesDefeated);
        }

        private void OnGameOver(GameOverMessage _) => StartCoroutine(ResetGameRoutine());

        private void OnAllEnemiesDefeated(AllEnemiesDefeatedMessage _) => StartCoroutine(ResetGameRoutine());

        private IEnumerator ResetGameRoutine()
        {
            yield return new WaitForSecondsRealtime(resetDelay);
            ReloadScene();
        }

        private static void ReloadScene()
        {
            Time.timeScale = 1f;
            EventBus.Clear();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
