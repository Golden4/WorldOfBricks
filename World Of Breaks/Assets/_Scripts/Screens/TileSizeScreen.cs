using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TileSizeScreen : ScreenBase<TileSizeScreen>
{
    public ChallengeLvlBtnUI pfSurvaveBtn;
    public ChallengeLvlBtnUI[] buttons;
    public Transform btnsParent;
    public Gradient gradient;
    public Gradient lockedGradient;
    // public Button[] tileSizeBtns;
    // public Text[] checkpointTexts;
    // public Text[] unlockItemTexts;

    public enum TileSize
    {
        Small,
        Medium,
        Big
    }

    static TileMapLockedSave _tileSizeLocked;

    static bool Loaded;

    public static TileMapLockedSave tileSizeLocked
    {
        get
        {
            if (_tileSizeLocked == null && !Loaded)
            {
                Loaded = true;

                Load();
            }

            return _tileSizeLocked;
        }
    }

    static int _tileSize = -1;

    public static TileSize tileSize
    {

        get
        {
            if (_tileSize == -1)
            {
                if (PlayerPrefs.HasKey("TileSize"))
                    _tileSize = PlayerPrefs.GetInt("TileSize");
                else
                    _tileSize = 0;
            }

            return (TileSize)_tileSize;
        }

        private set
        {
            _tileSize = (int)value;
            PlayerPrefs.SetInt("TileSize", _tileSize);
            PlayerPrefs.Save();
        }
    }

    public void SetTileSizeAndPlay(TileSize size)
    {
        tileSize = size;
        StartGame(true, false);
    }

    public override void OnInit()
    {
        base.OnInit();
        int TileLength = Enum.GetNames(typeof(TileSize)).Length;
        buttons = new ChallengeLvlBtnUI[TileLength];

        for (int i = 0; i < TileLength; i++)
        {
            int index = i;
            buttons[i] = Instantiate(pfSurvaveBtn).GetComponent<ChallengeLvlBtnUI>();
            buttons[i].transform.SetParent(btnsParent, false);
            buttons[i].GetComponent<Button>().onClick.AddListener(delegate
            {
                if (index > 0 && !tileSizeLocked.tileSizeLocked[index - 1] || index == 0)
                {
                    SetTileSizeAndPlay((TileSize)index);
                }
            });
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();

        // if (tileSizeLocked.tileSizeLocked[0])
        // {
        //     unlockItemTexts[0].gameObject.SetActive(true);
        //     unlockItemTexts[0].text = LocalizationManager.GetLocalizedText("medium_unlock");
        //     unlockItemTexts[0].GetComponentInParent<ButtonIcon>().EnableBtn(false);
        // }
        // else
        // {
        //     unlockItemTexts[0].gameObject.SetActive(false);
        //     unlockItemTexts[0].GetComponentInParent<ButtonIcon>().EnableBtn(true);
        // }

        // if (tileSizeLocked.tileSizeLocked[1])
        // {
        //     unlockItemTexts[1].gameObject.SetActive(true);
        //     unlockItemTexts[1].text = LocalizationManager.GetLocalizedText("big_unlock");
        //     unlockItemTexts[1].GetComponentInParent<ButtonIcon>().EnableBtn(false);
        // }
        // else
        // {
        //     unlockItemTexts[1].gameObject.SetActive(false);
        //     unlockItemTexts[1].GetComponentInParent<ButtonIcon>().EnableBtn(true);
        // }

        for (int i = 0; i < buttons.Length; i++)
        {
            string checkPointText = "";
            bool locked = false;
            string name = "";

            switch (i)
            {
                case 0:
                    name = LocalizationManager.GetLocalizedText("small");
                    break;
                case 1:
                    name = LocalizationManager.GetLocalizedText("medium");
                    break;
                case 2:
                    name = LocalizationManager.GetLocalizedText("big");
                    break;
                default:
                    break;
            }

            if (PlayerPrefs.HasKey("Checkpoint" + i))
            {
                if (PlayerPrefs.GetInt("Checkpoint" + i) != 0)
                {
                    checkPointText = LocalizationManager.GetLocalizedText("checkpoint") + ": " + PlayerPrefs.GetInt("Checkpoint" + i).ToString();
                }
                else
                {
                    checkPointText = LocalizationManager.GetLocalizedText("no_checkpoint");
                }
            }
            else
            {

                if (i > 0 && tileSizeLocked.tileSizeLocked[i - 1])
                {
                    if (i == 1)
                    {
                        checkPointText = LocalizationManager.GetLocalizedText("medium_unlock");
                    }

                    if (i == 2)
                    {
                        checkPointText = LocalizationManager.GetLocalizedText("big_unlock");
                    }
                    locked = true;
                }
            }

            Color color = gradient.Evaluate(i / (float)buttons.Length);

            if (locked)
            {
                color = lockedGradient.Evaluate(i / (float)buttons.Length);
            }

            buttons[i].ChangeChallengeBtnInfo(locked, name, checkPointText, (i + 1).ToString(), color);

        }

       /* for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.DOKill();
            buttons[i].transform.DOScale(1, .5f).ChangeStartValue(Vector3.zero).ChangeEndValue(Vector3.one).SetEase(Ease.OutCubic).SetDelay(.07f * i);
        }*/
    }

    public static void UnlockTile(int index)
    {
        _tileSizeLocked.tileSizeLocked[index] = false;
        SaveData();
    }

    public static void Load()
    {
        TileMapLockedSave data = JsonSaver.LoadData<TileMapLockedSave>("TileMapLocked");

        if (data == null)
        {
            _tileSizeLocked = new TileMapLockedSave();
            SaveData();

        }
        else
        {
            _tileSizeLocked = data;
        }
    }

    public void StartGame(bool newGame, bool isChallenge)
    {
        SceneController.LoadSceneWithFade(2);
        UIScreen.newGame = newGame;
        Game.isChallenge = isChallenge;
    }

    public static void SaveData()
    {
        if (_tileSizeLocked != null)
            JsonSaver.SaveData("TileMapLocked", _tileSizeLocked);
    }

    public class TileMapLockedSave
    {
        public bool[] tileSizeLocked = new bool[2] { true, true };
    }

}
