using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI HeavenCounterText;
    public TextMeshProUGUI HellCounterText;
    public TextMeshProUGUI ScoreText;

    private int score;
    private int heavens;
    private int hells;

    public static ScoreManager instance;
    private void Awake()
    {
        instance = this;
        updateScoreTexts();
    }
    public void PersonReached(Gate.type type)
    {
        switch (type)
        {
            case Gate.type.Heaven:
                heavens++;
                score += 100;
                break;
            case Gate.type.Hell:
                hells++;
                score -= 200;
                break;
        }
        updateScoreTexts();
    }
    public void DemonDispelled()
    {
        score += 11;
        updateScoreTexts();
    }
    private void updateScoreTexts()
    {
        HeavenCounterText.text = $"Heaven: {heavens}";
        HellCounterText.text = $"Hells: {hells}";
        ScoreText.text = $"Score: {score}";
    }
}
