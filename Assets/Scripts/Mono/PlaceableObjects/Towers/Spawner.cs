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
            Utils.GetManager<RunManager>().characterContainer
        ).GetComponent<Character>();
        character.SetStartPos(transform.position);
        character.faction = parentPlot.faction;
    }

    protected new bool Attack() {
        if (parentPlot.faction == Utils.GetManager<RunManager>().playerFaction) {
            SpawnCharacter();
        } else {
            if (powerRemaining > 0) {
                SpawnCharacter();
            } else if (!finishedWave) {
                finishedWave = true;
                Utils.GetManager<WaveManager>().hostileWaveSpawnersFinished++;
                return false;
            } else {
                return false;
            }
            powerRemaining--;
        }
        return true;
    }

    protected override void UpdateAttack() {
        if (canAttack) {
            if (Attack()) StartCoroutine(Reload());
        }
    }
}
