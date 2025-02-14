using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public static System.Random Random = new();

    private void Start() {
        if (instance) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
