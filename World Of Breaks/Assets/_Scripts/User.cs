using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class User
{

    public static int realCoinsCount;

    const string coinsKey = "coins";
    static int _coins;

    static bool coinsLoaded = false;

    public static int Coins
    {
        get
        {
            LoadCoins();
            return _coins;
        }

        private set
        {

            LoadCoins();

            SaveCoins(value);

            if (OnCoinChangedEvent != null)
                OnCoinChangedEvent(_coins, value);

            _coins = value;
        }
    }

    public static void AddCoin(int value)
    {
        Coins += value;
    }

    static void LoadCoins()
    {
        if (!coinsLoaded)
        {
            coinsLoaded = true;

            if (ZPlayerPrefs.HasKey(coinsKey))
            {
                _coins = ZPlayerPrefs.GetInt(coinsKey);
            }
        }
    }

    static void SaveCoins(int coinsCount)
    {
        ZPlayerPrefs.SetInt(coinsKey, coinsCount);
    }

    public static event Action<int, int> OnCoinChangedEvent;
    public static event Action<int> OnCoinChangedFailedEvent;

    static UserInfo dataUser;

    static bool Loaded;

    public static UserInfo GetInfo
    {
        get
        {
            if (dataUser == null && !Loaded)
            {
                Loaded = true;
                /*if (SaveLoadHelper.LoadFromFile<UserInfos> (out dataUser)) {
					
				} else {*/

                LoadUserInfoFromFileOrCreateNew();

                //dataUser = Resources.Load <UserInfo> ("Data/UserInfo");

                /*					UserInfos ui = new UserInfos ();
                                    ui.userData = new UserInfos.UserData[Database.Get.playersData.Length];

                                    for (int i = 0; i < ui.userData.Length; i++) {
                                        ui.userData [i] = new UserInfos.UserData ();
                                    }

                                //SaveLoadHelper.SaveToFile<UserInfos> (dataUser);
                                //}*/
            }

            return dataUser;
        }
    }

    static ChallDataInfo challengesData;

    static bool challengesLoaded;

    public static ChallDataInfo GetChallengesData
    {
        get
        {
            if (challengesData == null && !challengesLoaded)
            {
                challengesLoaded = true;

                LoadChallengesDataInfoFromFileOrCreateNew();
            }

            return challengesData;
        }
    }

    public static void SetPlayerIndex(int index)
    {
        GetInfo.curPlayerIndex = index;
        SaveUserInfo();
    }

    public static bool BuyWithCoin(int coinAmount)
    {
        if (Coins - coinAmount >= 0)
        {
            Coins -= coinAmount;
            if (CoinUI.Ins != null)
                CoinUI.Ins.AddCoin(-coinAmount);
            return true;
        }
        else
        {

            if (OnCoinChangedFailedEvent != null)
                OnCoinChangedFailedEvent(Coins);
            return false;
        }
    }

    public static bool HaveCoin(int coinAmount)
    {
        if (Coins - coinAmount >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void SaveUserInfo()
    {
        if (dataUser != null)
            JsonSaver.SaveData("UserInfo", dataUser);
    }

    public static void LoadUserInfoFromFileOrCreateNew()
    {
        UserInfo data = JsonSaver.LoadData<UserInfo>("UserInfo");
        if (data == null)
        {
            dataUser = new UserInfo();
            SaveUserInfo();

        }
        else
        {
            dataUser = data;

            if (dataUser.userData.Length < ItemsInfo.Get.playersData.Length)
            {

                UserInfo.UserData[] dataTemp = new UserInfo.UserData[ItemsInfo.Get.playersData.Length];

                for (int i = 0; i < ItemsInfo.Get.playersData.Length; i++)
                {
                    if (i < dataUser.userData.Length)
                    {
                        dataTemp[i] = dataUser.userData[i];
                    }
                    else
                        dataTemp[i] = new UserInfo.UserData();
                }
                dataUser.userData = dataTemp;
                SaveUserInfo();
            }

        }
    }

    public static void SaveChallengesData()
    {
        if (challengesData != null)
            JsonSaver.SaveData("ChallengesUserData", challengesData);
    }

    public static void LoadChallengesDataInfoFromFileOrCreateNew()
    {
        ChallDataInfo data = JsonSaver.LoadData<ChallDataInfo>("ChallengesUserData");

        challengesData = new ChallDataInfo();

        if (data != null)
        {
            challengesData.Combine(data.challData);
        }

        SaveChallengesData();
    }



}