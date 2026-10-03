using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI waveClearedText;

    private int score = 0;
    private int remainingTargets = 0;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        UpdateScoreUI();
        if (waveClearedText != null)
            waveClearedText.gameObject.SetActive(false);

    }

    public void SetTargetCount(int count) {
        remainingTargets = count;
        if (remainingTargets > 0 && waveClearedText != null)
            waveClearedText.gameObject.SetActive(false);
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
        if (waveClearedText != null) {
            waveClearedText.text = "WAVE CLEARED";
            waveClearedText.gameObject.SetActive(true);
        }
    }
}
