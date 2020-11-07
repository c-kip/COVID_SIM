using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents the virus
public class Virus
{
    public const int MAX_SPREAD = 100;
    public enum Stages
    {
        Asymptotic,
        Mild,
        Severe,
        Deadly
    };
    public static Dictionary<Stages, double> infSpreadRates = new Dictionary<Stages, double>() {
        {Stages.Asymptotic, 0.5},
        {Stages.Mild, 0.1},
        {Stages.Severe, 0.05},
        {Stages.Deadly, 0.01}
    };
    public static Dictionary<Stages, double> infStageIncRates = new Dictionary<Stages, double>() {
        {Stages.Asymptotic, 0},
        {Stages.Mild, 0.01},
        {Stages.Severe, 0.1},
        {Stages.Deadly, 0}
    };
    public static Dictionary<Stages, double> infStageDecRates = new Dictionary<Stages, double>() {
        {Stages.Asymptotic, 0},
        {Stages.Mild, 0.1},
        {Stages.Severe, 0.05},
        {Stages.Deadly, 0}
    };
}
