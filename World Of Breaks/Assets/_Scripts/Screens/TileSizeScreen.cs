using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileSizeScreen : ScreenBase {

	public Button[] tileSizeBtns;
	public Text[] checkpointTexts;
	public Text[] unlockItemTexts;

	public enum TileSize {
		Small,
		Medium,
		Big
	}

	static TileMapLockedSave _tileSizeLocked;

	static bool Loaded;

	public static TileMapLockedSave tileSizeLocked {
		get {
			if (_tileSizeLocked == null && !Loaded) {
				Loaded = true;

				Load ();
			}

			return _tileSizeLocked;
		}
	}

	static int _tileSize = -1;

	public static TileSize tileSize {
		
		get {
			if (_tileSize == -1) {
				if (PlayerPrefs.HasKey ("TileSize"))
					_tileSize = PlayerPrefs.GetInt ("TileSize");
				else
					_tileSize = 0;
			}

			return (TileSize)_tileSize;
		}

		private set {
			_tileSize = (int)value;
			PlayerPrefs.SetInt ("TileSize", _tileSize);
			PlayerPrefs.Save ();
		}
	}

	public void SetTileSize (TileSize size)
	{
		Debug.Log (size);
		tileSize = size;
		MenuScreen.Ins.StartGame (true, false);
	}

	public override void OnInit ()
	{
		base.OnInit ();

		for (int i = 0; i < tileSizeBtns.Length; i++) {
			int index = i;
			tileSizeBtns [i].onClick.AddListener (delegate {
				SetTileSize ((TileSize)index);
			});
		}

		for (int i = 0; i < 3; i++) {
			if (PlayerPrefs.HasKey ("Checkpoint" + i)) {
				if (PlayerPrefs.GetInt ("Checkpoint" + i) == 0) {
					checkpointTexts [i].gameObject.SetActive (false);
				} else {
					checkpointTexts [i].gameObject.SetActive (true);
					checkpointTexts [i].text = LocalizationManager.GetLocalizedText ("checkpoint") + ": " + PlayerPrefs.GetInt ("Checkpoint" + i).ToString ();
				}
			} else {
				checkpointTexts [i].gameObject.SetActive (false);
			}
		}

	}

	public override void OnActivate ()
	{
		base.OnActivate ();        

		if (tileSizeLocked.tileSizeLocked [0]) {
			unlockItemTexts [0].gameObject.SetActive (true);
			unlockItemTexts [0].text = LocalizationManager.GetLocalizedText ("medium_unlock");
			unlockItemTexts [0].GetComponentInParent<ButtonIcon> ().EnableBtn (false);
		} else {
			unlockItemTexts [0].gameObject.SetActive (false);
			unlockItemTexts [0].GetComponentInParent<ButtonIcon> ().EnableBtn (true);
		}

		if (tileSizeLocked.tileSizeLocked [1]) {
			unlockItemTexts [1].gameObject.SetActive (true);
			unlockItemTexts [1].text = LocalizationManager.GetLocalizedText ("big_unlock");
			unlockItemTexts [1].GetComponentInParent<ButtonIcon> ().EnableBtn (false);
		} else {
			unlockItemTexts [1].gameObject.SetActive (false);
			unlockItemTexts [1].GetComponentInParent<ButtonIcon> ().EnableBtn (true);
		}
	}

	public static void UnlockTile (int index)
	{
		_tileSizeLocked.tileSizeLocked [index] = false;
		SaveData ();
	}

	public static void Load ()
	{
		TileMapLockedSave data = JsonSaver.LoadData<TileMapLockedSave> ("TileMapLocked");

		if (data == null) {
			_tileSizeLocked = new TileMapLockedSave ();
			SaveData ();

		} else {
			_tileSizeLocked = data;
		}
	}

	public static void SaveData ()
	{
		if (_tileSizeLocked != null)
			JsonSaver.SaveData ("TileMapLocked", _tileSizeLocked);
	}

	public class TileMapLockedSave {
		public bool[] tileSizeLocked = new bool[2] { true, true };
	}

}
