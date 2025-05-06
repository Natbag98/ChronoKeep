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

        // Test
        foreach (Mod mod in RunManager.instance.testMods) character.AddMod(mod);

        character.SetStartPos(transform.position);
        character.faction = parentPlot.faction;
        character.characterSO = character_to_spawn;
    }

    protected new bool Attack() {
        Dictionary<int, SOCharacter> character_costs = new();
        foreach (SOCharacter character in potentialCharactersToSpawn) {
            character_costs.Add(character.powerRequired, character);
        }

        if (parentPlot.faction == GameManager.instance.Game.PlayerFaction) {
            SOCharacter character = Utils.Choice(
                (
                    from cost
                    in character_costs.Keys
                    where cost  <= GameManager.instance.Game.GetResources()[GameManager.Resources.ManPower]
                    select character_costs[cost]
                ).ToList()
            );

            if (GameManager.instance.Game.SpendResources(GameManager.Resources.ManPower, character_costs.FirstOrDefault(x => x.Value == character).Key)) {
                SpawnCharacter(character);
                return true;
            }
            return false;
        } else if (partOfHostileWave) {
            if (powerRemaining > 0) {
                SOCharacter character = Utils.Choice(
                    (
                        from cost
                        in character_costs.Keys
                        where cost  <= powerRemaining
                        select character_costs[cost]
                    ).ToList()
                );

                SpawnCharacter(character);
                powerRemaining -= character_costs.FirstOrDefault(x => x.Value == character).Key;
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
