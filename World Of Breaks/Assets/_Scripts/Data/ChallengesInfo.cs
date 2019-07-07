using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ChallengesInfo : ScriptableObject
{
    public ChallengeInfo[] challengesData;

    [System.Serializable]
    public class ChallengeInfo
    {
        public int lifeCount;
        public int ballCount;
    }
}
