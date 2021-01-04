﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

public class Ads : MonoBehaviour
{
    public AudioSource music;
    private string adUnitId = "ca-app-pub-3940256099942544/1033173712";
    private InterstitialAd interstitial;

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        LoadInterstitialAd();
    }

    public void ShowInterstitialAd()
    {
        if (interstitial.IsLoaded())
        {
            music.Pause();
            interstitial.Show();
        }
    }

    private void LoadInterstitialAd()
    {
        if(this.interstitial != null)
        {
            this.interstitial.Destroy();
        }
        this.interstitial = new InterstitialAd(this.adUnitId);
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        AdRequest request = new AdRequest.Builder().Build();
        this.interstitial.LoadAd(request);
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        music.Play();
        LoadInterstitialAd();
    }
}
