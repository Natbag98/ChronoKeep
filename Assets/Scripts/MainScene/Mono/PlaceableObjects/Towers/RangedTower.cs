using System.Collections.Generic;
using UnityEngine;

public class RangedTower : Tower {
    protected override void GetTarget() {
        List<Character> characters_in_range = new();
        foreach (Plot plot in GetPlotsInRange()) characters_in_range.AddRange(plot.GetCharacters());
        if (characters_in_range.Count > 0) target = Utils.Choice(characters_in_range).transform;
    }

    protected override void Attack() {
        Debug.Log(target);
    }
}
