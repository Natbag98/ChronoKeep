using TMPro;
using UnityEngine;

public class CharacterButton : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;

    [HideInInspector] public Spawner spawner;
    [HideInInspector] public SOCharacter character;

    public void _Button_Clicked() {
        if (GameManager.instance.Game.SpendResources(GameManager.Resources.ManPower, character.powerRequired)) {
            spawner.charactersToSpawn[character]++;
        }
    }
    
    private void Update() {
        text.text = $"{character.displayName}\n\n{spawner.charactersToSpawn[character]}";
    }
}
