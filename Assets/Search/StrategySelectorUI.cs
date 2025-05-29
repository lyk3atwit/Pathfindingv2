using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategySelectorUI : MonoBehaviour
{
    public Dropdown strategyDropdown;
    public SimplePathfinder pathfinder;

    void Start()
    {
        strategyDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void OnDropdownChanged(int index)
    {
        pathfinder.strategy = (SimplePathfinder.SearchStrategy)index;
    }
}
