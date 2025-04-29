using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PostRunSceneUIManager : MonoBehaviour {
    public static PostRunSceneUIManager instance;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI skillText;

    public void _Button_ContinueButtonClicked() {
        SceneManager.LoadScene("GameScene");
    }

    private void Start() {
        instance = this;
    }

    private void Update() {
        scoreText.text = $"Score: {GameManager.instance.scoreLastRun}";
        skillText.text = $"Skill Earned: {GameManager.instance.skillLastRun}";
    }
}
