﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Database {

	static ItemsInfo data;

	static bool Loaded;

	public static ItemsInfo Get {
		get {
			if (data == null && !Loaded) {
				Loaded = true;
				ItemsInfo pi = Resources.Load<ItemsInfo> ("Data/Database");

				data = pi;
			}

			return data;
		}
	}

    static ChallengesInfo dataChall;

    static bool LoadedChall;

    public static ChallengesInfo GetChall
    {
        get
        {
            if (dataChall == null && !LoadedChall)
            {
                LoadedChall = true;
                ChallengesInfo pi = Resources.Load<ChallengesInfo>("Data/ChallengesInfo");

                dataChall = pi;
            }

            return dataChall;
        }
    }

}
