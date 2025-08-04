using UnityEngine;

public class EnemyDot : MonoBehaviour {
    public float height;
    
    [HideInInspector] public GameObject character;

    private void Update() {
        if (character.gameObject == null) {
            Destroy(gameObject);
        }

        transform.position = new(
            character.transform.position.x,
            height,
            character.transform.position.z
        );
    }
}
