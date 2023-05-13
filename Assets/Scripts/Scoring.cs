using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoring : MonoBehaviour
{
    public static Scoring instance;
    private void Awake()
    {
        instance = this;
    }

    float score;
    [SerializeField] TMP_Text scoreText;

    private void Start()
    {
        score = 0f;
        scoreText.text = Mathf.CeilToInt(score).ToString();
    }
    public void IncreaseScore(float scoreAmount)
    {
        if (scoreAmount < 0f)
            scoreAmount = 0f;

        score += scoreAmount;
        scoreText.text = Mathf.CeilToInt(score).ToString();
    }
}
