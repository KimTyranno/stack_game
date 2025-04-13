using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField]
    private GameObject mainPanel;

    [Header("InGame")]
    [SerializeField]
    private TextMeshProUGUI textCurrentScore;

    [Header("GameOver")]
    [SerializeField]
    private GameObject textNewRecord;
    [SerializeField]
    private GameObject imageCrown;
    [SerializeField]
    private TextMeshProUGUI textHighScore;
    [SerializeField]
    private GameObject textTouchToRestart;

    public void GameStart()
    {
        mainPanel.SetActive(false);
        textCurrentScore.gameObject.SetActive(true);
    }

    public void UpdateScore(int score)
    {
        textCurrentScore.text = score.ToString();
    }

    public void GameOver(bool isNewRecord)
    {
        if (isNewRecord)
        {
            textNewRecord.SetActive(true);
        }
        else
        {
            imageCrown.SetActive(true);
            textHighScore.text = PlayerPrefs.GetInt("HighScore").ToString();
            textHighScore.gameObject.SetActive(true);
        }

        textTouchToRestart.SetActive(true);
    }
}
