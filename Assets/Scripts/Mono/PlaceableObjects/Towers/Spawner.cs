using System.Collections.Generic;
using UnityEngine;

public class Spawner : Tower {
    [Header("Spawner")]
    [SerializeField] private SOCharacter[] potentialCharactersToSpawn;

    private int powerRemaining;
    private bool finishedWave;

    public void SpawnHostileWave(int power) {
        powerRemaining = power;
        finishedWave = false;
    }

    public void SpawnCharacter() {
        Character character = Instantiate(
            Utils.Choice(potentialCharactersToSpawn).prefab,
            transform.position,
            Quaternion.identity,
            RunManager.instance.characterContainer
        ).GetComponent<Character>();
        character.faction = parentPlot.faction;
    }

    protected override void Attack() {
        if (parentPlot.faction == RunManager.instance.playerFaction) {
            SpawnCharacter();
        } else {
            if (powerRemaining > 0) {
                SpawnCharacter();
            } else if (!finishedWave) {
                finishedWave = true;
                WaveManager.instance.hostileWaveSpawnersFinished++;
            }
            powerRemaining--;
        }
    }

    protected override void UpdateAttack() {
        if (canAttack) {
            Attack();
            StartCoroutine(Reload());
        }
    }
}
