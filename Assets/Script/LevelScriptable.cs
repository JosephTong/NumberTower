using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NumberTower", menuName = "ScriptableObjects/Level", order = 1)]
public class LevelScriptable : ScriptableObject
{
    public float PlayerHp = 1;
    public List<NumberTowerColume> Columes = new List<NumberTowerColume>();
}

