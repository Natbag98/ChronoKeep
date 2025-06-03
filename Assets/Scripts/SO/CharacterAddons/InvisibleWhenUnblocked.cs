using UnityEngine;

[CreateAssetMenu(fileName = "InvisibleWhenUnblocked", menuName = "SOCharacterAddon/InvisibleWhenUnblocked")]
public class InvisibleWhenUnblocked : SOCharacterAddon {
    public override void AddonAwake(Character character) {
        character.invisible = true;
    }

    public override void AddonUpdate(Character character) {
        character.invisible = !character.blocked;
    }
}
