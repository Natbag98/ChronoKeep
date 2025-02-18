using System.Collections.Generic;
using UnityEngine;

public class Spawner : Tower {
    [Header("Spawner")]
    [SerializeField] private SOCharacter[] potentialCharactersToSpawn;

    [HideInInspector] public int powerRemaining;

    public void SpawnCharacter() {
        Instantiate(Utils.Choice(potentialCharactersToSpawn).prefab, transform.position, Quaternion.identity, RunManager.instance.characterContainer);
    }

    protected override void Attack() {
        if (parentPlot.faction == RunManager.instance.playerFaction) {
            SpawnCharacter();
        } else {
            if (powerRemaining > 0) SpawnCharacter();
            powerRemaining--;
        }
    }

    protected override void UpdateAttack() {
        if (canAttack) {
            if (canAttack) {
                Attack();
                StartCoroutine(Reload());
            }
        }
    }
}
