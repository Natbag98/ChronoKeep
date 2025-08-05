using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : Tower {
    [Header("Spawner")]
    [SerializeField] private SOCharacter[] potentialCharactersToSpawn;

    private int powerRemaining;
    public bool spawning { private set; get; } = false;
    public bool partOfHostileWave { private set; get; } = false;
    public Dictionary<SOCharacter, int> charactersToSpawn = new();

    private void WaveEnd(object _, EventArgs __) {
        charactersToSpawn = new();
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

        if (parentPlot.faction == GameManager.instance.Game.PlayerFaction && GameManager.instance.Game.GetResources()[GameManager.Resources.ManPower] > 0) {
            SOCharacter character = Utils.Choice(
                (
                    from cost
                    in character_costs
                    where cost.Value <= GameManager.instance.Game.GetResources()[GameManager.Resources.ManPower]
                    select cost.Key
                ).ToList()
            );

            if (GameManager.instance.Game.SpendResources(GameManager.Resources.ManPower, character_costs[character])) {
                SpawnCharacter(character);
                return true;
            }
            return false;

        } else if (partOfHostileWave) {
            if (!parentPlot.visibleToPlayer) {
                Debug.Log("Plot not visble to player");
                spawning = false;
                partOfHostileWave = false;
                return false;
            } else if (powerRemaining > 0) {
                SOCharacter character = Utils.Choice(
                    (
                        from cost
                        in character_costs
                        where cost.Value <= powerRemaining
                        select cost.Key
                    ).ToList()
                );

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
