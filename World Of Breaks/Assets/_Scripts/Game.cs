using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Game
{

    public static event Action OnGameStarted;

    public static bool isPause;
    public static bool isChallenge;

    public static int curChallengeIndex;
    public static int curChallengeGroupIndex;

    public static ChallengesInfo.ChallengeInfo curChallengeInfo;

    public static int ballTryingIndex = -1;
    public static int gamesPlayed;

    public static void OnGameStartedCall()
    {
        if (Game.OnGameStarted != null)
            Game.OnGameStarted();
    }

}
