namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class ToolClutterInteraction : MonoBehaviour
    {

    [SerializeField] private GameObject mindTool;
    [SerializeField] private int patienceValue;
    [SerializeField] private int indoctrinationValue;
    [SerializeField] private MoodEnum moodInpact;
    [SerializeField] private int workTime;

    public GameObject MindTool => mindTool;
    public int PatienceValue => patienceValue;
    public int IndoctrinationValue => indoctrinationValue;
    public MoodEnum MoodInpact => moodInpact;
    public int WorkTime => workTime;

    }
}