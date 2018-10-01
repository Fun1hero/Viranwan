using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public Wave[] waves;
	public Enemy enemy;

	Wave currentWave;
	int currentWaveNumber;

	int enemyRemainingToSpawn;
	int enemiesRemainingAlive;
	float nextSpawnTime;

	LivingEntity playerEntity;
	Transform playerT; 
	MapGenerator map;

	float timeBetweenCampingChecks = 2;
	float campThresholdDistance = 1.5f;
	float nextCampCheckTime;
	Vector3 campPositionOld;
	bool isCamping;
	bool isDisabled;

	public event System.Action<int> OnNewWave;

	void Start(){
		playerEntity = FindObjectOfType<Player>();
		playerT = playerEntity.transform;
		playerEntity.onDeath += OnPlayerDeath;

		nextCampCheckTime = timeBetweenCampingChecks+ Time.time;
		campPositionOld = playerT.position;

		map = FindObjectOfType<MapGenerator>();
		NextWave();
	}

	void Update (){
		if (!isDisabled) {
			if (Time.time > nextCampCheckTime) {
				nextCampCheckTime += Time.time + timeBetweenCampingChecks;

				isCamping = (Vector3.Distance (playerT.position, campPositionOld) < campThresholdDistance);
				campPositionOld = playerT.position;
			}

			if (enemyRemainingToSpawn > 0 && Time.time > nextSpawnTime) {
				enemyRemainingToSpawn--;
				nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
				StartCoroutine (SpawnEnemy ());
			}
		}
	}

	IEnumerator SpawnEnemy (){
		float spawnDelay = 1;
		float flashSpeed = 4;
		Transform spawnTile = map.GetRandomOpenTile();
		if (isCamping){
			spawnTile = map.GetTileFromPosition(playerT.position);
		}
		Material tileMat = spawnTile.GetComponent<Renderer>().material;
		Color initialTileColor = tileMat.color;
		Color flashColor = Color.red;

		float spawnTimer = 0;

		while (spawnTimer < spawnDelay){
			tileMat.color = Color.Lerp(initialTileColor, flashColor, Mathf.PingPong(spawnTimer * flashSpeed, 1));

			spawnTimer += Time.deltaTime;
			yield return null;
		}

		Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.onDeath += OnEnemyDeath;
	}

	void OnEnemyDeath(){
		enemiesRemainingAlive--;

		if (enemiesRemainingAlive == 0){
			NextWave();
		}
	}

	void ReserPlayerPosition (){
		playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
	}

	void OnPlayerDeath (){
		isDisabled = true;
	}

	void NextWave (){
		currentWaveNumber++;

		if (currentWaveNumber - 1 < waves.Length) {
			currentWave = waves [currentWaveNumber - 1];

			enemyRemainingToSpawn = currentWave.enemyCount;
			enemiesRemainingAlive = enemyRemainingToSpawn;

			if (OnNewWave != null) OnNewWave(currentWaveNumber);
		}
		ReserPlayerPosition();
	}

	[System.Serializable]
	public class Wave {
		public int enemyCount;
		public float timeBetweenSpawns;
	}
}
