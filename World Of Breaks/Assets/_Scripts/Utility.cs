using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public static class Utility
{

    public static void LoadFromResources(string pathToObject, Vector3 position, Transform parent = null, float destroyTime = -1)
    {
        LoadFromResources(pathToObject, position, Quaternion.identity, parent, destroyTime);
    }

    public static void LoadFromResources(string pathToObject, Vector3 position, Quaternion rotation, Transform parent = null, float destroyTime = -1)
    {
        GameObject particle = (GameObject)Resources.Load(pathToObject);
        GameObject ps = MonoBehaviour.Instantiate(particle);
        ps.transform.SetParent(parent, false);
        ps.transform.position = position;

        if (rotation != Quaternion.identity)
            ps.transform.rotation = rotation;

        if (destroyTime != -1)
        {
            MonoBehaviour.Destroy(ps.gameObject, destroyTime);
        }

    }

    public static void Invoke(MonoBehaviour t, float delay, UnityAction action, bool realtime = false)
    {
        t.StartCoroutine(InvokeCoroutine(action, delay, realtime));
    }

    static IEnumerator InvokeCoroutine(UnityAction action, float delay, bool realtime = false)
    {
        if (!realtime)
            yield return new WaitForSeconds(delay);
        else
            yield return new WaitForSecondsRealtime(delay);

        action.Invoke();
    }

    public static void AnimateValue(Text text, float fromValue, float toValue, float speed)
    {
        text.StartCoroutine(AnimateValueCoroutine(text, fromValue, toValue, speed));
    }

    static IEnumerator AnimateValueCoroutine(Text text, float fromValue, float toValue, float speed)
    {
        float t = 0;

        while (t < 1)
        {
            text.text = Mathf.RoundToInt(Mathf.Lerp(fromValue, toValue, t)).ToString();

            t += Time.deltaTime * speed;
            yield return null;
        }

        text.text = Mathf.RoundToInt(toValue).ToString();
    }

    public static void CoinsAnimate(MonoBehaviour m, GameObject prefab, Transform parent, int coinsCount, Vector2 fromPos, Vector2 toPos, float time, AnimationCurve speedCurve, Action coinInTarget)
    {
        GameObject[] objs = new GameObject[coinsCount];

        for (int i = 0; i < coinsCount; i++)
        {
            objs[i] = MonoBehaviour.Instantiate(prefab) as GameObject;
            objs[i].transform.SetParent(parent, false);
            objs[i].transform.SetAsFirstSibling();
        }

        for (int i = 0; i < coinsCount; i++)
        {
            MonoBehaviour.Destroy(objs[i], time + .5f + i * .1f);
            MoveTo(m, objs[i].transform, fromPos, toPos, time, speedCurve, i * .1f, coinInTarget);
        }
    }

    public static void CoinsAnimateRadial(MonoBehaviour m, GameObject prefab, Transform parent, int coinsCount, Vector2 fromPos, Vector2 toPos, float radius, float time, AnimationCurve speedCurve, Action coinInTarget)
    {
        GameObject[] objs = new GameObject[coinsCount];

        for (int i = 0; i < coinsCount; i++)
        {
            objs[i] = MonoBehaviour.Instantiate(prefab) as GameObject;
            objs[i].transform.SetParent(parent, false);
            objs[i].transform.SetAsLastSibling();

            objs[i].transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        }

        Vector2[] randPos = new Vector2[coinsCount];


        for (int i = 0; i < coinsCount; i++)
        {
            randPos[i] = new Vector2(UnityEngine.Random.Range(radius, -radius), UnityEngine.Random.Range(radius, -radius)) + fromPos;
            MoveTo(m, objs[i].transform, fromPos, randPos[i], time * UnityEngine.Random.Range(.5f, 1f), speedCurve, 0, null);
        }

        for (int i = 0; i < coinsCount; i++)
        {
            GameObject go = objs[i];
            MoveTo(m, objs[i].transform, randPos[i], toPos, time, speedCurve, time, delegate
            {

                if (coinInTarget != null)
                    coinInTarget.Invoke();

                MonoBehaviour.Destroy(go);

            });
        }

    }

    public static void MoveTo(MonoBehaviour m, Transform obj, Vector2 fromPos, Vector2 toPos, float time, AnimationCurve speedCurve, float delay, Action coinInTarget)
    {
        m.StartCoroutine(MoveToCoroutine(obj, fromPos, toPos, time, speedCurve, delay, coinInTarget));
    }

    static IEnumerator MoveToCoroutine(Transform obj, Vector2 fromPos, Vector2 toPos, float time, AnimationCurve speedCurve, float delay, Action coinInTarget)
    {
        yield return new WaitForSecondsRealtime(delay);

        float t = 0;

        while (t < 1)
        {
            obj.position = Vector3.Lerp(fromPos, toPos, speedCurve.Evaluate(t));
            t += Time.fixedUnscaledDeltaTime / time;
            yield return new WaitForSecondsRealtime(Time.fixedUnscaledDeltaTime);

        }

        if (coinInTarget != null)
            coinInTarget.Invoke();

        obj.position = toPos;

    }

    public static Quaternion DirectionToRotation(Vector2 direction, bool inverse = false)
    {
        float angle;

        if (!inverse)
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        else
            angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;

        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public static void FadeUI(Transform parent, float endValue, float duration, bool ignoreTimeScale = false)
    {
        foreach (Image item in parent.GetComponentsInChildren<Image>())
        {
            Color color = item.color;
            item.DOFade(endValue, duration).ChangeStartValue(color).SetUpdate(ignoreTimeScale);
        }

        foreach (Text item in parent.GetComponentsInChildren<Text>())
        {
            Color color = item.color;
            item.DOFade(endValue, duration).ChangeStartValue(color).SetUpdate(ignoreTimeScale);
        }
    }

    public static void FadeUIFrom(Transform parent, float startValue, float duration, bool ignoreTimeScale = false)
    {
        foreach (Image item in parent.GetComponentsInChildren<Image>())
        {
            item.DOFade(startValue, duration).From().SetUpdate(ignoreTimeScale);
        }
    }

    public static void FadeUITo(Transform parent, float endValue, float duration, bool ignoreTimeScale = false)
    {
        foreach (Image item in parent.GetComponentsInChildren<Image>())
        {
            item.DOFade(endValue, duration).SetUpdate(ignoreTimeScale);
        }

        foreach (Text item in parent.GetComponentsInChildren<Text>())
        {
            item.DOFade(endValue, duration).SetUpdate(ignoreTimeScale);
        }
    }

    public static void FadeSpritesFrom(Transform parent, float startValue, float duration, bool ignoreTimeScale = false)
    {
        foreach (SpriteRenderer item in parent.GetComponentsInChildren<SpriteRenderer>())
        {
            item.DOFade(startValue, duration).From().SetUpdate(ignoreTimeScale);
        }
    }

    public static void FadeSpritesTo(Transform parent, float endValue, float duration, bool ignoreTimeScale = false)
    {
        foreach (SpriteRenderer item in parent.GetComponentsInChildren<SpriteRenderer>())
        {
            item.DOFade(endValue, duration).SetUpdate(ignoreTimeScale);
        }
    }

}

[System.Serializable]
public class Vector2I
{
    public int x;
    public int y;

    public Vector2I()
    {
        this.x = 0;
        this.y = 0;
    }

    public Vector2I(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Vector2I vecA, Vector2I vecB)
    {
        if (System.Object.ReferenceEquals(vecA, null) || System.Object.ReferenceEquals(vecB, null))
        {
            return false;
        }

        return vecA.x == vecB.x && vecA.y == vecB.y;
    }

    public static bool operator !=(Vector2I vecA, Vector2I vecB)
    {
        if (System.Object.ReferenceEquals(vecA, null) || System.Object.ReferenceEquals(vecB, null))
        {
            return true;
        }

        return vecA.x != vecB.x || vecA.y != vecB.y;
    }

    public static Vector2I operator +(Vector2I vecA, Vector2I vecB)
    {
        Vector2I vec = new Vector2I(vecA.x + vecB.x, vecA.y + vecB.y);
        return vec;
    }

    public static Vector2I operator -(Vector2I vecA, Vector2I vecB)
    {
        Vector2I vec = new Vector2I(vecA.x - vecB.x, vecA.y - vecB.y);
        return vec;
    }

    public static Vector2I operator *(Vector2I vecA, int num)
    {
        Vector2I vec = new Vector2I(vecA.x * num, vecA.y * num);
        return vec;
    }

    public override string ToString()
    {
        return string.Format("x={0}, y={1}", x, y);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != typeof(Vector2I))
            return false;
        Vector2I other = (Vector2I)obj;
        return x == other.x && y == other.y;
    }


    public override int GetHashCode()
    {
        unchecked
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
    }

    public Vector3 ToVector3(float y = 0)
    {
        return new Vector3(this.x, y, this.y);
    }
}
