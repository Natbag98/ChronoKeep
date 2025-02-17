using UnityEngine;

public class Spawner : Tower {
    [Header("Spawner")]
    [SerializeField] private SOCharacter[] potentialCharactersToSpawn;

    public void SpawnCharacter() {
        Instantiate(Utils.Choice(potentialCharactersToSpawn).prefab, transform.position, Quaternion.identity, RunManager.instance.characterContainer);
    }
}
