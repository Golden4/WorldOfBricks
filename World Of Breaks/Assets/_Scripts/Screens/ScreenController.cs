using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public static ScreenController Ins;

    public enum GameScreen
    {
        Menu,
        Shop,
        BuyCoin,
        TileSize,
        UI,
        GameOver,
        Pause,
        Continue,
        Challeges,
        ChallegesGroup,
        ChallegesResult,
        Settings,
    }

    public enum CurScene
    {
        Menu,
        Game
    }

    public CurScene curScene;

    public List<ScreenList> screensList = new List<ScreenList>();

    [System.Serializable]
    public class ScreenList
    {
        public ScreenBase screen;
    }

    public static GameScreen curActiveScreen;

    private Stack<ScreenBase> screenStack = new Stack<ScreenBase>();

    public static event System.Action<ScreenBase> OnChangeScreenEvent;

    void Awake()
    {
        Ins = this;

        for (int i = 0; i < screensList.Count; i++)
        {
            screensList[i].screen.OnInit();
        }
    }

    void Start()
    {
        Time.timeScale = 1;
        DeactivateAll();
        ActivateScreen(0);
    }

    public static T GetScreen<T>() where T : ScreenBase
    {
        for (int i = 0; i < Ins.screensList.Count; i++)
        {
            if (Ins.screensList[i].GetType() == typeof(T))
            {
                return (T)Ins.screensList[i].screen;
            }
        }

        Debug.LogError("Not Found Screen: " + typeof(T).Name);

        return default(T);
    }

    /*	public static void ActivateScreen<T> (T screen) where T:ScreenBase
	{
		for (int i = 0; i < Ins.screensList.Length; i++) {
			if (typeof(T) == Ins.screensList [i].GetType ()) {
				ActivateScreen (i);
			}
		}
	}*/

    public void ActivateScreen(GameScreen screen)
    {
        curActiveScreen = (GameScreen)screen;
        int index = screensList.FindIndex(x => x.screen.screenType == screen);
        ActivateScreen(index);
    }

    public void ActivateScreen(int screen)
    {
        ScreenBase screenBase = screensList[screen].screen;

        if (screenStack.Count > 0)
        {
            if (screenBase.disableScreenUnderneath)
            {
                foreach (var screenT in screenStack)
                {
                    screenT.OnDeactivate();
                    screenT.gameObject.SetActive(false);
                    if (screenT.disableScreenUnderneath)
                        break;
                }
            }

            // var topCanvas = screenBase.GetComponent<Canvas>();
            // var previousCanvas = menuStack.Peek().GetComponent<Canvas>();
            // topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
        }

        screenBase.gameObject.SetActive(true);
        screenBase.OnActivate();
        screenStack.Push(screenBase);

        // for (int i = 0; i < screensList.Count; i++)
        // {
        //     if (i == screen)
        //     {
        //         screensList[i].screen.Activate();
        //     }
        //     else
        //     {
        //         screensList[i].screen.Deactivate();
        //     }
        // }

        if (OnChangeScreenEvent != null)
        {
            OnChangeScreenEvent.Invoke(screensList[screen].screen);
        }

    }

    public void DeactivateAll()
    {
        for (int i = 0; i < screensList.Count; i++)
        {
            screensList[i].screen.OnDeactivate();
            screensList[i].screen.gameObject.SetActive(false);
        }
    }

    public void DeactivateScreen(ScreenBase screenBase)
    {
        if (screenStack.Count == 0)
        {
            Debug.LogErrorFormat(screenBase, "{0} cannot be closed because menu stack is empty", screenBase.GetType());
            return;
        }

        if (screenStack.Peek() != screenBase)
        {
            Debug.LogErrorFormat(screenBase, "{0} cannot be closed because it is not on top of stack", screenBase.GetType());
            return;
        }

        DeactivateTopScreen();
        screenBase.OnDeactivate();
    }

    public void DeactivateTopScreen()
    {
        var instance = screenStack.Pop();

        instance.gameObject.SetActive(false);
        instance.OnDeactivate();
        // Re-activate top menu
        // If a re-activated menu is an overlay we need to activate the menu under it
        foreach (var menu in screenStack)
        {
            menu.gameObject.SetActive(true);
            menu.OnActivate();
            if (menu.disableScreenUnderneath)
                break;
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < screensList.Count; i++)
        {
            screensList[i].screen.OnCleanUp();
        }
    }

    void OnApplicationQuit()
    {
    }

    void OnApplicationFocus(bool pause)
    {
        if (pause && curActiveScreen == GameScreen.UI && curScene == CurScene.Game)
            ActivateScreen(GameScreen.Pause);
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause && curActiveScreen == GameScreen.UI && curScene == CurScene.Game)
        {
            ActivateScreen(GameScreen.Pause);
        }
    }

}
