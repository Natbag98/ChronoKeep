using System.Collections.Generic;
using UnityEngine;

public class Spawner : Tower {
    [Header("Spawner")]
    [SerializeField] private SOCharacter[] potentialCharactersToSpawn;

    private int powerRemaining;
    public bool spawning { private set; get; } = false;
    public bool partOfHostileWave { private set; get; } = false;

    public void SpawnHostileWave(int power) {
        powerRemaining = power;
        spawning = true;
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
            } else if (spawning) {
                spawning = false;
                partOfHostileWave = false;
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
