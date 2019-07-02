using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo {
	
	public UserData[] _userData;

	public UserData[] userData {
		get {
			return _userData;
		}

		set {
			_userData = value;
		}
	}

	public int curPlayerIndex = 0;

	[System.Serializable]
	public class UserData {
		public bool bought;
	}

	public void ResetValues ()
	{
		for (int i = 0; i < userData.Length; i++) {
			userData [i].bought = (i == 0);
		}
	}

	public bool AllCharactersBought ()
	{
		for (int i = 0; i < userData.Length; i++) {
			if (!userData [i].bought) {
				return false;
			}
		}
		return true;
	}

	public UserInfo ()
	{
		_userData = new UserData[Database.Get.playersData.Length];

		for (int i = 0; i < _userData.Length; i++) {
			_userData [i] = new UserData ();
		}

		curPlayerIndex = 0;
		ResetValues ();
	}

	public UserInfo (UserData[] userData, int curPlayerIndex)
	{
		this.userData = userData;
		this.curPlayerIndex = curPlayerIndex;
	}
}
