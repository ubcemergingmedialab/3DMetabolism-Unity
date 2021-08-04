﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Node", menuName = "Node")]
public class NodeSO : ScriptableObject
{
    public string Label;
    public Vector3 Position;
    public string QID;
    public Quaternion Rotation;
    [TextArea(10, 100)]
    public string Description;
    public string GibbsFreeEnergy;
    [TextArea(10, 100)]
    public string EnzymeRequired;
}
