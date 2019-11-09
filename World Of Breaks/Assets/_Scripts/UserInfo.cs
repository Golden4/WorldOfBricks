using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo
{

    public UserData[] _userData;

    public UserData[] userData
    {
        get
        {
            return _userData;
        }

        set
        {
            _userData = value;
        }
    }

    public int curPlayerIndex = 0;

    [System.Serializable]
    public class UserData
    {
        public bool bought;
    }

    public void ResetValues()
    {
        for (int i = 0; i < userData.Length; i++)
        {
            userData[i].bought = (i == 0);
        }
    }

    public bool AllCharactersBought()
    {
        for (int i = 0; i < userData.Length; i++)
        {
            if (!userData[i].bought)
            {
                return false;
            }
        }
        return true;
    }

    public UserInfo()
    {
        _userData = new UserData[ItemsInfo.Get.playersData.Length];

        for (int i = 0; i < _userData.Length; i++)
        {
            _userData[i] = new UserData();
        }

        curPlayerIndex = 0;
        ResetValues();
    }

    public UserInfo(UserData[] userData, int curPlayerIndex)
    {
        this.userData = userData;
        this.curPlayerIndex = curPlayerIndex;
    }

    public int GetCurPlayerIndex()
    {
        if (Game.ballTryingIndex > -1)
            return Game.ballTryingIndex;
        else
            return curPlayerIndex;
    }



}

[System.Serializable]
public class ChallDataInfo
{

    [System.Serializable]
    public class ChallengeGroup
    {
        public bool locked = true;
        public int[] data;
    }

    public ChallengeGroup[] _challData;

    public ChallengeGroup[] challData
    {
        get
        {
            return _challData;
        }

        set
        {
            _challData = value;
        }
    }

    public int GetCountData(int groupIndex)
    {

        int count = 0;

        for (int i = 0; i < challData[groupIndex].data.Length; i++)
        {
            if (challData[groupIndex].data[i] > 0)
                count++;
        }

        return count;
    }

    public int GetSummData(int groupIndex)
    {
        int summ = 0;

        for (int i = 0; i < challData[groupIndex].data.Length; i++)
        {
            summ += challData[groupIndex].data[i];
        }

        return summ;
    }

    public int GetValue(int challengeGroupIndex, int challengeIndex)
    {
        return _challData[challengeGroupIndex].data[challengeIndex];
    }

    public int GetCurrentValue()
    {
        return _challData[Game.curChallengeGroupIndex].data[Game.curChallengeIndex];
    }

    public void SetCurrentValue(int value)
    {
        _challData[Game.curChallengeGroupIndex].data[Game.curChallengeIndex] = value;
    }

    public ChallDataInfo()
    {
        _challData = new ChallengeGroup[ChallengesInfo.GetChall.challengesGroups.Count];

        for (int i = 0; i < _challData.Length; i++)
        {
            _challData[i] = new ChallengeGroup();

            if (i == 0)
                _challData[i].locked = false;

            _challData[i].data = new int[ChallengesInfo.GetChall.challengesGroups[i].challengesData.Count];
            for (int j = 0; j < _challData[i].data.Length; j++)
            {
                _challData[i].data[j] = 0;
            }
        }

        ResetValues();
    }

    public void Combine(ChallengeGroup[] _data)
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _challData[i].locked = _data[i].locked;
            for (int j = 0; j < _data[i].data.Length; j++)
            {
                if (_challData[i].data.Length > j)
                    _challData[i].data[j] = _data[i].data[j];
            }
        }
    }

    public void ResetValues()
    {
        for (int i = 0; i < _challData.Length; i++)
        {
            _challData[i].data = new int[ChallengesInfo.GetChall.challengesGroups[i].challengesData.Count];
            for (int j = 0; j < _challData[i].data.Length; j++)
            {
                _challData[i].data[j] = 0;
            }
        }
    }
}