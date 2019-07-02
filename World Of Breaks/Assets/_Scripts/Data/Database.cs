using System.Collections;
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
}
