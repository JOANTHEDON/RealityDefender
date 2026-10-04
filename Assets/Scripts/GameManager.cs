using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject waveClearedGameObject;

    private int score = 0;
    private int remainingTargets = 0;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        UpdateScoreUI();
        if (waveClearedGameObject != null)
            waveClearedGameObject.gameObject.SetActive(false);

    }

    public void SetTargetCount(int count) {
        remainingTargets = count;
        if (remainingTargets > 0 && waveClearedGameObject != null)
            waveClearedGameObject.gameObject.SetActive(false);
    }

    public void TargetDestroyed() {
        score += 10;
        remainingTargets--;
        UpdateScoreUI();

        if (remainingTargets <= 0) {
            WaveCLeared();
        }
    }

    private void UpdateScoreUI() {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
    }

    private void WaveCLeared() {
        if (waveClearedGameObject != null) {

            waveClearedGameObject.SetActive(true);
        }
    }
}
