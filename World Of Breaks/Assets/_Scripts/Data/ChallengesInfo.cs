using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ChallengesInfo : ScriptableObject
{
    static ChallengesInfo dataChall;

    static bool loaded;

    public static ChallengesInfo GetChall
    {
        get
        {
            if (dataChall == null && !loaded)
            {
                loaded = true;
                ChallengesInfo pi = Resources.Load<ChallengesInfo>("Data/ChallengesInfo");

                dataChall = pi;
            }

            return dataChall;
        }
    }

    public ChallengeInfo[] challengesData;

    public List<ChallengeGroup> challengesGroups;

    public ChallengeInfo[] GetCurrentChallengesData()
    {
        return challengesGroups[Game.curChallengeGroupIndex].challengesData.ToArray();
    }

    [System.Serializable]
    public class ChallengeInfo
    {
        public int lifeCount;
        public int ballCount;
        public float maxScoreMultiplayer = 1;
    }

    [System.Serializable]
    public class ChallengeGroup
    {
        public string name;
        public Texture2D mapTexture;
        public List<ChallengeInfo> challengesData;
    }
}
