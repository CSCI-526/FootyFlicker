using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ScoreTableController : MonoBehaviour
{
    public float gameDuration = 60f;
    public Coin2DController coin;

    TextMeshProUGUI timerText;
    TextMeshProUGUI coinText;
    TextMeshProUGUI instructionsText; 
    TextMeshProUGUI meterDisplayText;
    TextMeshProUGUI gameOverText;
    int currentScore = 0;
    float timeRemaining;
    bool isGameOver = false;
    Canvas canvas;

    public bool IsGameOver => isGameOver;
    public int CurrentScore => currentScore;

    void Start()
    {
        timeRemaining = gameDuration;
        if (coin == null)
        {
            coin = FindFirstObjectByType<Coin2DController>();
        }   
        CreateUI();
    }

    // creates all UI elements at runtime so we don't need prefabs
    void CreateUI()
    {
        canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("GameCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        timerText = MakeText("Timer", new Vector2(-20, -20), 32);
        timerText.rectTransform.anchorMin = new Vector2(1, 1);
        timerText.rectTransform.anchorMax = new Vector2(1, 1);
        timerText.rectTransform.pivot = new Vector2(1, 1);

        coinText = MakeText("ScoreCoins", new Vector2(0, -20), 24, new Vector2(500, 100));
        coinText.rectTransform.anchorMin = new Vector2(0.5f, 1);
        coinText.rectTransform.anchorMax = new Vector2(0.5f, 1);
        coinText.rectTransform.pivot = new Vector2(0.5f, 1);

        instructionsText = MakeText("Instructions", new Vector2(20, 20), 24, new Vector2(450, 80));
        instructionsText.rectTransform.anchorMin = Vector2.zero;
        instructionsText.rectTransform.anchorMax = Vector2.zero;
        instructionsText.rectTransform.pivot = Vector2.zero;

        meterDisplayText = MakeText("Meter", new Vector2(0, 100), 24, new Vector2(500, 80));
        meterDisplayText.rectTransform.anchorMin = new Vector2(0.5f, 0);
        meterDisplayText.rectTransform.anchorMax = new Vector2(0.5f, 0);
        meterDisplayText.rectTransform.pivot = new Vector2(0.5f, 0);

        gameOverText = MakeText("GameOver", Vector2.zero, 32, new Vector2(500, 250));
        gameOverText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        gameOverText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        gameOverText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        gameOverText.gameObject.SetActive(false);
    }

    //Uses TextMeshProGUI to create and format text elements in the game
    TextMeshProUGUI MakeText(string name, Vector2 pos, int size, Vector2? customSize = null)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform, false);
        var text = obj.AddComponent<TextMeshProUGUI>();
        text.fontSize = size;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        return text;
    }

    void Update()
    {
        if (isGameOver) {
            return;
        }
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            EndGame("TIME'S UP!");
        }
        UpdateUI();
    }

    //Changes the score and coin amounts whenever a player shoots. Also creates the shooting meter.
    void UpdateUI()
    {
        timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}s";

        if (coin != null)
        {
            coinText.text = $"Nickels: {coin.NickelsRemaining}\nDimes: {coin.DimesRemaining}\nQuarters: {coin.QuartersRemaining}\nScore: {currentScore} cents";
            instructionsText.text = "[1/2/3] Select coin type\n[SPACE] Hold to aim, release to shoot!";

            if (coin.IsAiming)
            {
                // build a text-based meter like [-------|--O--------]
                int pos = Mathf.RoundToInt((coin.MeterValue + 1) * 10);
                string meter = "[";
                for (int i = 0; i < 21; i++)
                    meter += i == 10 ? "|" : i == pos ? "O" : "-";
                meterDisplayText.text = $"AIM: {meter}]";
            }  
        } 
    }

    public void AddScore(int points)
    {
        currentScore += points;
    } 
    public void OnMiss() { }
    public void OnBlocked() { }
    public void OnOutOfCoins() {
        EndGame("YOU'RE OUT OF COINS!");
    }

    void EndGame(string reason)
    {
        isGameOver = true;
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = $"GAME OVER!\n{reason}\n\nFinal Score: {currentScore} cents\n";
    }

    
}
