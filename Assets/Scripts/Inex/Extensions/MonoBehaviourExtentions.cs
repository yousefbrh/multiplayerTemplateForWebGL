using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class MonoBehaviourExtentions
{
    #region Delayed Invoke

    public static void CallWithDelay(this MonoBehaviour mono, Action action, float delay)
    {
        mono.StartCoroutine(TheCoroutine(action, delay));
    }

    public static void DelayedCall(this MonoBehaviour mono, Action action, float delay)
    {
        mono.StartCoroutine(TheCoroutine(action, delay));
    }

    public static IEnumerator TheCoroutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    #endregion


    #region Shuffling

    // shuffle array extension method
    public static void Shuffle<T>(this T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            T temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    // shuffle list extension method
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    #endregion


    #region Transform

    public static void DestroyChildren(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            Object.Destroy(child.gameObject);
        }
    }

    #endregion

    #region Navmesh

    public static bool HasReachedDestination(this NavMeshAgent agent)
    {
        if (agent.pathPending) return false;
        if (!(agent.remainingDistance <= agent.stoppingDistance)) return false;
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        {
            return true;
        }

        return false;
    }

    #endregion
}