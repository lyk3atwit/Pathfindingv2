using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SimplePathfinder;

public class HeuristicSelectorUI : MonoBehaviour
{
    public Dropdown heuristicDropdown;
    public SimplePathfinder pathfinder;

    void Start()
    {
        heuristicDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void OnDropdownChanged(int index)
    {
        pathfinder.heuristic = (SimplePathfinder.HeuristicType)index;
    }
}
