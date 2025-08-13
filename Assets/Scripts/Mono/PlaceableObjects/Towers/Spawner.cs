using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : Tower {
    [Header("Spawner")]
    [SerializeField] private SOCharacter[] potentialCharactersToSpawn;

    private int powerRemaining;
    public bool spawning { private set; get; } = false;
    public bool partOfHostileWave { private set; get; } = false;
    public Dictionary<SOCharacter, int> charactersToSpawn = new();
    [HideInInspector] public List<SOCharacter> charactersToSpawnList = new();

    private void WaveEnd(object _, EventArgs __) {
        charactersToSpawn = new();
        charactersToSpawnList = new();
        foreach (SOCharacter character in potentialCharactersToSpawn) charactersToSpawn.Add(character, 0);
    }

    public void SpawnHostileWave(int power) {
        powerRemaining = power;
        spawning = true;
        partOfHostileWave = true;
    }

    public void SpawnCharacter(SOCharacter character_to_spawn) {
        Character character = Instantiate(
            character_to_spawn.prefab,
            transform.position,
            Quaternion.identity,
            RunManager.instance.characterContainer
        ).GetComponent<Character>();

        character.SetStartPos(transform.position);
        character.faction = parentPlot.faction;
        character.characterSO = character_to_spawn;
        character.spawner = this;

        EnemyDot enemy_dot = Instantiate(MainSceneUIManager.instance.enemyDotPrefab).GetComponent<EnemyDot>();
        enemy_dot.character = character.gameObject;
    }

    protected new bool Attack() {
        Dictionary<SOCharacter, int> character_costs = new();
        foreach (SOCharacter character in potentialCharactersToSpawn) {
            character_costs.Add(character, character.powerRequired);
        }

        if (parentPlot.faction == GameManager.instance.Game.PlayerFaction) {
            if (charactersToSpawnList.Count != 0 || (from spawn_count in from to_spawn in charactersToSpawn.Values select to_spawn where spawn_count > 0 select spawn_count).ToList().Count > 0) {
                if (charactersToSpawnList.Count == 0 && charactersToSpawn.Count != 0) {
                    foreach (var pair in charactersToSpawn) {
                        for (int i = 0; i < pair.Value; i++) charactersToSpawnList.Add(pair.Key);
                    }
                    charactersToSpawnList.Reverse();
                    charactersToSpawn = new();
                }

                SpawnCharacter(charactersToSpawnList[0]);
                charactersToSpawnList.RemoveAt(0);
                return true;
            }
        } else if (partOfHostileWave) {
            if (!parentPlot.visibleToPlayer && GetComponentInParent<Plot>().faction == GameManager.instance.Game.BaseFactions[^1]) {
                Debug.Log("Barbarian plot not visble to player");
                spawning = false;
                partOfHostileWave = false;
                return false;
            } else if (powerRemaining > 0) {
                List<SOCharacter> potential_characters = (
                    from cost
                    in character_costs
                    where cost.Value <= powerRemaining
                    select cost.Key
                ).ToList();
                if (potential_characters.Count == 0) {
                    powerRemaining = 0;
                    return false;
                }
                SOCharacter character = Utils.Choice(potential_characters);

                SpawnCharacter(character);
                powerRemaining -= character_costs[character];
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

    protected override void Start() {
        charactersToSpawn = new();
        foreach (SOCharacter character in potentialCharactersToSpawn) charactersToSpawn.Add(character, 0);
        WaveManager.instance.waveEnd += WaveEnd;
        base.Start();
    }
}
