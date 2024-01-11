using System.Collections;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class HeartReward : MonoBehaviour
    {
        #region Fields

        [Tooltip("How much hearts did player get for touching the heart")]
        [SerializeField] private int minReward = 1, maxReward = 3;

        [SerializeField] private ParticleSystem winParticles;

        [SerializeField] private Transform[] rewardSpawnPoints;

        [SerializeField] private BoxCollider2D collider2D;
        
        #endregion

        private void Awake() => collider2D ??= GetComponent<BoxCollider2D>();

        private void Start() => transform.position = rewardSpawnPoints[Random.Range(0, rewardSpawnPoints.Length)].position;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                int changedValue = Random.Range(minReward, maxReward);
                int finalCount = PlayerPrefs.GetInt("Hearts", 0) + changedValue;
                
                PlayerPrefs.SetInt("Hearts", finalCount);

                EventsManager.OnHeartsUpdated?.Invoke(changedValue);
                EventsManager.LastChangeValue = changedValue;
                winParticles.Play();

                collider2D.enabled = false;
                
                StartCoroutine(ReloadSceneAfterDelay(1f)); 
            }
        }
        
        private IEnumerator ReloadSceneAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}