using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    private GameController gameController;
    public EnemyWave[] Waves;
    public Transform currentSpawn;

    public UnityEvent OnWaveSpawn;
    public UnityEvent OnWavesDone;

    public int activeEnemies;
    public int deathSFX;

    public void Start()
    {
        gameController = FindObjectOfType<GameController>();
        gameController.currentEnemyCount = 0;
    }

    public void SpawnWave(int waveNumber)
    {
        if(Waves.Length - 1 < waveNumber)
        {
            OnWavesDone.Invoke();
            return;
        }

        if (waveNumber > 0)
        {
            //TODO add sound manager
            OnWaveSpawn.Invoke();
        }
       
        activeEnemies = Waves[waveNumber].EnemyNumber;
        FindObjectOfType<GameController>().currentEnemyCount = activeEnemies;

        for (int i = 0; i <= Waves[waveNumber].EnemyNumber - 1; i++)
        {
            GameObject enemy = Instantiate(selectRandomEnemy(waveNumber), currentSpawn.position, Quaternion.identity);
            enemy.GetComponent<NPCController>().currentWaypoint = findClosestWayPoint(enemy.transform);
        }
    }
    public void OnEnemyDeath()
    {
        //Play death sound
        AudioManager.Instance.PlaySFX(deathSFX);

        activeEnemies -= 1;
        gameController = FindObjectOfType<GameController>();
        gameController.currentEnemyCount -= 1;
    }

    private GameObject selectRandomEnemy(int currentWave)
    {
        int enemyIndex = Random.Range(0, Waves[currentWave].Enemies.Length);
        return Waves[currentWave].Enemies[enemyIndex];
    }

    private Waypoint findClosestWayPoint(Transform enemyTransform)
    {
        Vector3 enemyPos = enemyTransform.position;

        Waypoint closestPoint =
            FindObjectsOfType<Waypoint>().OrderBy(
                w => (w.transform.position - enemyPos).sqrMagnitude).First();

        return closestPoint;
    }
}
