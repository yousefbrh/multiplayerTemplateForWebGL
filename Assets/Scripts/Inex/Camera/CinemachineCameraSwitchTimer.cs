using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
[AddComponentMenu("Inex/Camera/Set Cinemachine Camera Switch Timer")]
public class CinemachineCameraSwitchTimer : MonoBehaviour
{
    [PropertySpace(0, 10)] public float timeToSwitch = 3f;

    private void Awake()
    {
        FindObjectOfType<CinemachineBrain>().m_DefaultBlend.m_Time = timeToSwitch;
    }
}
