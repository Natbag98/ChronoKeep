using System.Collections.Generic;
using UnityEngine;

public class Spawner : Tower {
    [Header("Spawner")]
    [SerializeField] private SOCharacter[] potentialCharactersToSpawn;

    private int powerRemaining;
    private bool finishedWave;
    private bool partOfHostileWave = false;

    public void SpawnHostileWave(int power) {
        powerRemaining = power;
        finishedWave = false;
        partOfHostileWave = true;
    }

    public void SpawnCharacter() {
        Character character = Instantiate(
            Utils.Choice(potentialCharactersToSpawn).prefab,
            transform.position,
            Quaternion.identity,
            Utils.GetManager<RunManager>().characterContainer
        ).GetComponent<Character>();

        // Test
        foreach (Mod mod in Utils.GetManager<RunManager>().testMods) character.AddMod(mod);

        character.SetStartPos(transform.position);
        character.faction = parentPlot.faction;
    }

    protected new bool Attack() {
        if (parentPlot.faction == GameManager.instance.Game.PlayerFaction) {
            SpawnCharacter();
            return true;
        } else if (partOfHostileWave) {
            if (powerRemaining > 0) {
                SpawnCharacter();
                powerRemaining--;
                return true;
            } else if (!finishedWave) {
                finishedWave = true;
                partOfHostileWave = false;
                Utils.GetManager<WaveManager>().hostileWaveSpawnersFinished++;
                return false;
            } else {
                return false;
            }
        }
        return false;
    }

    protected override void UpdateAttack() {
        if (canAttack) {
            if (Attack()) StartCoroutine(Reload());
        }
    }
}
