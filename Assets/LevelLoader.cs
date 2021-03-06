﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class LevelLoader : MonoBehaviour
{
    public GameObject canvas;
    private ScreenManager screenManager;
    public GameMusic music;
    public textTimer timer;
    public GameObject bombPieces;
    public GameObject pieceShooter;
    public PieceShooter pieceShooterComp;
    public GameObject shootTapZone;
    public GameOverUI gameOverUI;
    public Score score;
    public LevelIndicator levelIndicator;
    public List<Reflector> pieceReflectors;
    public List<Level> levels;
    private int currLevel = -1;
    private float startDelaySec;
    private GameObject bomb;
    private DataStorage dataStorage;

    public void Start()
    {
        screenManager = GameObject.FindGameObjectWithTag("ScreenManager").GetComponent<ScreenManager>();
        dataStorage = GameObject.FindGameObjectWithTag("DataStorage").GetComponent<DataStorage>();
        pieceShooterComp = pieceShooter.GetComponent<PieceShooter>();
    }

    public void LoadLevel(int levelIndex, float startDelaySec)
    {
        if(levelIndex == 0)
        {
            score.Reset();
        }
        currLevel = levelIndex;
        ResetCurrentLevel();
        StartCoroutine(StartCurrentLevelAfterDelay(startDelaySec));
    }

    public void StartCurrentLevelAfterDelaySec(float delaySec)
    {
        this.startDelaySec = delaySec;
        StartCoroutine(StartCurrentLevelAfterDelay(delaySec));
    }

    public IEnumerator StartCurrentLevelAfterDelay(float delaySec)
    {
        yield return new WaitForSeconds(delaySec);
        StartCurrentLevel();
    }

    public void StartCurrentLevel()
    {
        timer.UnPause();
        pieceShooter.SetActive(false);
        pieceShooter.GetComponent<PieceShooter>().SetShootingEnabled(true);
        bomb.SendMessage("StartBomb");
        music.Play();
        pieceShooter.SetActive(true);
        Level level = levels[currLevel];
        pieceShooterComp.SetAngleChangeMode(level.pieceShooterAngleChangeMode);
        pieceShooterComp.SetAngles(level.pieceShooterAngles);
        pieceShooterComp.Init();
        if (!Application.isEditor)
        {
            AnalyticsEvent.LevelStart(currLevel + 1, new Dictionary<string, object>
            {
                { "score", score.GetScore() }
            });
        }
    }

    public void ResetCurrentLevel()
    {
        Level level = levels[currLevel];
        pieceShooter.SetActive(false);
        foreach (GameObject prevBomb in GameObject.FindGameObjectsWithTag("bomb"))
        {
            Destroy(prevBomb);
        }
        bomb = Instantiate(level.bomb);
        bomb.transform.SetParent(canvas.transform, false);
        timer.Init(bomb.GetComponent<Detonator>(), bomb.GetComponentInChildren<BombDefuzer>());
        timer.gameObject.SetActive(true);
        timer.setTime(level.secondsOnTimer);
        timer.Pause();
        foreach (Transform child in bombPieces.transform)
        {
            Destroy(child.gameObject);
        }
        bombPieces.SetActive(true);
        if (this.startDelaySec > 0)
        {
            pieceShooter.GetComponent<PieceShooter>().SetShootingEnabled(false);
        }
        shootTapZone.SetActive(true);
        gameOverUI.Hide();
        music.Reset();
        levelIndicator.Set(this.GetCurrentLevel() + 1, this.LevelCount());
    }

    public void LoadNextLevelAndStartAfterDelay(float delaySec)
    {
        if (currLevel < LevelCount() - 1)
        {
            LoadLevel(++currLevel, delaySec);
        }
        else
        {
            currLevel = 0;
            dataStorage.SaveLevel(0);
            screenManager.ShowGameWonScreen();
        }
    }

    public int GetCurrentLevel()
    {
        return currLevel;
    }

    public int LevelCount()
    {
        return levels.Count;
    }

    [System.Serializable]
    public class Level
    {
        public GameObject bomb;
        public float secondsOnTimer;
        public float[] pieceShooterAngles;
        public PieceShooter.AngleChangeMode pieceShooterAngleChangeMode = PieceShooter.AngleChangeMode.ON_SHOOT;
    }
}
