using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    string starKey = "StarCount";
    string dashKey = "MaxDashCount";
    public int CurrentStarCount { get; set; }
    public int MaxDashCount { get; set; }

    // Start is called before the first frame update
    private void Awake()
    {
        CurrentStarCount = PlayerPrefs.GetInt(starKey);
        MaxDashCount = PlayerPrefs.GetInt(dashKey) + 1;
    }

   public void SetStarCount(int starCount)
   {
        PlayerPrefs.SetInt(starKey, starCount);
   }

    public void UpdateMaxDashCount()
    {
        PlayerPrefs.SetInt(dashKey, MaxDashCount + 1);
    }
}
