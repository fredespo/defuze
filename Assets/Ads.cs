﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

public class Ads : MonoBehaviour
{
    public bool adsEnabled = true;
    public AudioSource music;
    public string testAdId = "ca-app-pub-3940256099942544/1033173712";
    public string androidAdId = "ca-app-pub-8136996048120593/8224216593";
    private InterstitialAd interstitial;

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        LoadInterstitialAd();
    }

    public void ShowInterstitialAd()
    {
        if(!this.adsEnabled)
        {
            return;
        }

        if(interstitial != null && interstitial.IsLoaded())
        {
            music.Pause();
            interstitial.Show();
        }
    }

    private void LoadInterstitialAd()
    {
        if(Application.isEditor || !this.adsEnabled)
        {
            return;
        }

        if(this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        this.interstitial = new InterstitialAd(androidAdId);
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
