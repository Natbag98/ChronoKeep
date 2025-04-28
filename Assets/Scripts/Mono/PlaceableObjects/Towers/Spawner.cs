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
            RunManager.instance.characterContainer
        ).GetComponent<Character>();

        // Test
        foreach (Mod mod in RunManager.instance.testMods) character.AddMod(mod);

        character.SetStartPos(transform.position);
        character.faction = parentPlot.faction;
    }

    protected new bool Attack() {
        if (parentPlot.faction == GameManager.instance.Game.PlayerFaction) {
            if (GameManager.instance.Game.SpendResources(GameManager.Resources.ManPower, 1)) {
                SpawnCharacter();
                return true;
            }
            return false;
        } else if (partOfHostileWave) {
            if (powerRemaining > 0) {
                SpawnCharacter();
                powerRemaining--;
                return true;
            } else if (!finishedWave) {
                finishedWave = true;
                partOfHostileWave = false;
                WaveManager.instance.hostileWaveSpawnersFinished++;
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

    protected override void DestroySelf() {
        if (!finishedWave) WaveManager.instance.hostileWaveSpawnersFinished++;
        base.DestroySelf();
    }
}
