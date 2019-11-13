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
    public int groupsUnlocked = 0;

    [System.Serializable]
    public class ChallengeGroup
    {
        public int levelsUnlocked = 0;
        public int[] data;

        public bool IsLevelLocked(int index)
        {
            return index > levelsUnlocked;
        }

        public bool UnlockNextLevel(int curIndex)
        {
            if (curIndex + 1 < data.Length)
            {
                if (IsLevelLocked(curIndex + 1))
                {
                    levelsUnlocked = curIndex + 1;
                    return true;
                }
            }

            return false;
        }
    }

    public bool UnlockNextGroup(int curGroupIndex)
    {
        if (curGroupIndex + 1 < challData.Length)
        {
            if (IsGroupLocked(curGroupIndex + 1))
            {
                groupsUnlocked = curGroupIndex + 1;
                return true;
            }
        }

        return false;
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


    public bool IsGroupLocked(int index)
    {
        return index > groupsUnlocked;
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
                groupsUnlocked = 0;

            _challData[i].levelsUnlocked = 0;
            _challData[i].data = new int[ChallengesInfo.GetChall.challengesGroups[i].challengesData.Count];
            for (int j = 0; j < _challData[i].data.Length; j++)
            {
                _challData[i].data[j] = 0;
            }
        }

        ResetValues();
    }

    public void Combine(ChallDataInfo _data)
    {
        groupsUnlocked = _data.groupsUnlocked;

        for (int i = 0; i < _data.challData.Length; i++)
        {
            _challData[i].levelsUnlocked = _data.challData[i].levelsUnlocked;
            for (int j = 0; j < _data.challData[i].data.Length; j++)
            {
                if (_challData[i].data.Length > j)
                    _challData[i].data[j] = _data.challData[i].data[j];
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