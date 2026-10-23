// ============================================================
// Title:       HubProgress
// Version:     1.0
// Written by:  Ashar
//              ai use: a little help from claude (anthropic) when stuck, owner Ashar
// Description: The welcome stall goes for good once its window is
//              closed. After the first shard (the level 1 boss's
//              portal piece) a shard portal stall takes its place,
//              with a 3x3 grid showing a white cube per shard.
//              Goes on the IntroMenu container (always active).
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubProgress : MonoBehaviour
{
    //welcome
    public Button welcomeCloseButton;   //the welcome window's Close button
    public GameObject welcomeStall;     //IntroWall

    //shards
    public GameObject shardStall;       //ShardPortal, same spot as the welcome stall
    public GameObject[] shardCubes;     //9 white cubes, one per shard, in grid order

    //STARTING PROCEDURES
    void Start()
    {
        welcomeCloseButton.onClick.AddListener(CloseWelcome);
        Refresh();
    }

    //WELCOME
    void CloseWelcome()
    {
        ProfileManager.Profile.WelcomeSeen = true;
        ProfileManager.Save();
        Debug.Log("Welcome closed, welcome stall removed.");
        Refresh();
    }

    //STALLS
    void Refresh()
    {
        PlayerProfile p = ProfileManager.Profile;
        int shards = p.ShardCount;

        welcomeStall.SetActive(!p.WelcomeSeen);
        shardStall.SetActive(p.WelcomeSeen && shards > 0);   //replaces the welcome stall after the first shard

        for (int i = 0; i < shardCubes.Length; i++)
            shardCubes[i].SetActive(i < shards);
    }
}
