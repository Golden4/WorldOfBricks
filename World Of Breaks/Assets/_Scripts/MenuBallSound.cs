// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MenuBallSound : MonoBehaviour
// {

//     public int itemId = -1;

//     void Start()
//     {
//         Debug.Log("dsadsad");
//         AnimationEvent eventAnim = new AnimationEvent();
//         eventAnim.functionName = "PlayBallSound";
//         eventAnim.time = 1;
//         eventAnim.intParameter = itemId;
//         GetComponent<Animation>().clip.AddEvent(eventAnim);
//     }

//     public void PlayBallSound(int itemId)
//     {
//         if (ScreenController.curActiveScreen == ScreenController.GameScreen.Shop)
//         {
//             if (itemId == ShopScreen.Ins.curActiveItem)
//                 AudioManager.PlaySound(ItemsInfo.Get.playersData[ShopScreen.Ins.curActiveItem].hitSound);
//         }
//         else if (ScreenController.curActiveScreen == ScreenController.GameScreen.Menu)
//         {
//             if (itemId == -1)
//                 AudioManager.PlaySound(ItemsInfo.Get.playersData[User.GetInfo.curPlayerIndex].hitSound);
//         }
//     }
// }
