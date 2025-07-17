using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime; // Needed for Photon Player class
using TMPro;

public class Leaderboard : MonoBehaviourPunCallbacks
{
    public GameObject playersHolder;

    [Header("Options")]
    public float refreshRate = 1f;

    [Header("UI")]
    public GameObject[] slots;

    [Space]
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] nameTexts;

    private void Start()
    {
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
    }

    private void Refresh()
    {
        // Hide all slots initially
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        // Sort players by score (assuming score is stored in CustomProperties with key "score")
        var sortedPlayerList = PhotonNetwork.PlayerList
            .OrderByDescending(p => GetScore(p))
            .ToList();

        int i = 0;
        foreach (var player in sortedPlayerList)
        {
            if (i >= slots.Length) break;

            slots[i].SetActive(true);

            string name = string.IsNullOrEmpty(player.NickName) ? "unnamed" : player.NickName;
            nameTexts[i].text = name;
            scoreTexts[i].text = GetScore(player).ToString();

            i++;
        }
    }

    private int GetScore(Player player)
    {
        if (player.CustomProperties.TryGetValue("score", out object scoreObj))
        {
            return Convert.ToInt32(scoreObj);
        }
        return 0;
    }

    private void Update()
    {
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}

