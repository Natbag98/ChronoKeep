using UnityEngine;
using UnityEngine.UI;

public class EnemyDot : MonoBehaviour {
    [SerializeField] private Image dot;
    [SerializeField] private float height;
    
    [HideInInspector] public GameObject character;

    private void Start() {
        if (character.GetComponent<Character>().faction == GameManager.instance.Game.PlayerFaction) {
            dot.color = new(0, 0, 255);
        } else {
            dot.color = new(255, 0, 0);
        }
    }

    private void Update() {
        if (character == null) {
            Destroy(gameObject);
        } else {
            transform.position = new(
                character.transform.position.x,
                height,
                character.transform.position.z
            );
        }
    }
}
