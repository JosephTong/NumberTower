using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NumberTowerColume{
    public List<NumberTowerRoomData> Rooms = new List<NumberTowerRoomData>();
}

[System.Serializable]
public enum NumberTowerOperations
{
    Enemy = 0 ,
    Add ,
    Minus ,
    Multiply , 
    Divide
}

[System.Serializable]
public class NumberTowerRoomData 
{
    public float Number = 0;
    public Color BGColor;
    public NumberTowerOperations Operations = NumberTowerOperations.Add;
    public GameObject Prefab;

}
