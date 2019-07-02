using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Utility {

	public static void LoadFromResources (string pathToObject, Vector3 position, Transform parent = null, float destroyTime = -1)
	{
		LoadFromResources (pathToObject, position, Quaternion.identity, parent, destroyTime);
	}

	public static void LoadFromResources (string pathToObject, Vector3 position, Quaternion rotation, Transform parent = null, float destroyTime = -1)
	{
		GameObject particle = (GameObject)Resources.Load (pathToObject);
		GameObject ps = MonoBehaviour.Instantiate (particle);
		ps.transform.SetParent (parent, false);
		ps.transform.position = position;

		if (rotation != Quaternion.identity)
			ps.transform.rotation = rotation;

		if (destroyTime != -1) {
			MonoBehaviour.Destroy (ps.gameObject, destroyTime);
		}

	}

	public static void Invoke (MonoBehaviour t, float delay, Action action)
	{
		t.StartCoroutine (InvokeCoroutine (action, delay));
	}

	static IEnumerator InvokeCoroutine (Action action, float delay)
	{
		yield return new WaitForSeconds (delay);

		action.Invoke ();
	}

	public static void AnimateValue (Text text, float fromValue, float toValue, float speed)
	{
		text.StartCoroutine (AnimateValueCoroutine (text, fromValue, toValue, speed));
	}

	static IEnumerator AnimateValueCoroutine (Text text, float fromValue, float toValue, float speed)
	{
		float t = 0;

		while (t < 1) {
			text.text = Mathf.RoundToInt (Mathf.Lerp (fromValue, toValue, t)).ToString (); 

			t += Time.deltaTime * speed;
			yield return null;
		}

		text.text = Mathf.RoundToInt (toValue).ToString ();
	}

	public static void CoinsAnimate (MonoBehaviour m, GameObject prefab, Transform parent, int coinsCount, Vector2 fromPos, Vector2 toPos, float time, AnimationCurve speedCurve, Action coinInTarget)
	{
		GameObject[] objs = new GameObject[coinsCount];

		for (int i = 0; i < coinsCount; i++) {
			objs [i] = MonoBehaviour.Instantiate (prefab) as GameObject;
			objs [i].transform.SetParent (parent, false);
			objs [i].transform.SetAsFirstSibling ();
		}

		for (int i = 0; i < coinsCount; i++) {
			MonoBehaviour.Destroy (objs [i], time + .5f + i * .1f);
			MoveTo (m, objs [i].transform, fromPos, toPos, time, speedCurve, i * .1f, coinInTarget);
		}

	}

	public static void MoveTo (MonoBehaviour m, Transform obj, Vector2 fromPos, Vector2 toPos, float time, AnimationCurve speedCurve, float delay, Action coinInTarget)
	{
		m.StartCoroutine (MoveToCoroutine (obj, fromPos, toPos, time, speedCurve, delay, coinInTarget));
	}

	static IEnumerator MoveToCoroutine (Transform obj, Vector2 fromPos, Vector2 toPos, float time, AnimationCurve speedCurve, float delay, Action coinInTarget)
	{
		yield return new WaitForSeconds (delay);

		float t = 0;

		while (t < 1) {
			obj.position = Vector3.Lerp (fromPos, toPos, speedCurve.Evaluate (t));
			t += Time.deltaTime / time;
			yield return null;

		}

		coinInTarget.Invoke ();
		obj.position = toPos;

	}

	public class CoinAnimation {

	}

}

[System.Serializable]
public class Vector2I {
	public int x;
	public int y;

	public Vector2I ()
	{
		this.x = 0;
		this.y = 0;
	}

	public Vector2I (int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static bool operator== (Vector2I vecA, Vector2I vecB)
	{
		if (System.Object.ReferenceEquals (vecA, null) || System.Object.ReferenceEquals (vecB, null)) {
			return false;
		}

		return vecA.x == vecB.x && vecA.y == vecB.y;
	}

	public static bool operator!= (Vector2I vecA, Vector2I vecB)
	{
		if (System.Object.ReferenceEquals (vecA, null) || System.Object.ReferenceEquals (vecB, null)) {
			return true;
		}

		return vecA.x != vecB.x || vecA.y != vecB.y;
	}

	public static Vector2I operator+ (Vector2I vecA, Vector2I vecB)
	{
		Vector2I vec = new Vector2I (vecA.x + vecB.x, vecA.y + vecB.y);
		return vec;
	}

	public static Vector2I operator- (Vector2I vecA, Vector2I vecB)
	{
		Vector2I vec = new Vector2I (vecA.x - vecB.x, vecA.y - vecB.y);
		return vec;
	}

	public static Vector2I operator* (Vector2I vecA, int num)
	{
		Vector2I vec = new Vector2I (vecA.x * num, vecA.y * num);
		return vec;
	}

	public override string ToString ()
	{
		return string.Format ("x={0}, y={1}", x, y);
	}

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Vector2I))
			return false;
		Vector2I other = (Vector2I)obj;
		return x == other.x && y == other.y;
	}


	public override int GetHashCode ()
	{
		unchecked {
			return x.GetHashCode () ^ y.GetHashCode ();
		}
	}

	public Vector3 ToVector3 (float y = 0)
	{
		return new Vector3 (this.x, y, this.y);
	}
}
