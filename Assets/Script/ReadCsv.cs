using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using AdvancedEditorTools.Attributes;
using System.IO;

public class ReadCsv : MonoBehaviour
{
    //[SerializeField] private NumberTowerGameManager m_NumberTowerGameManager;
    [SerializeField] private string m_LevelPath = "";
    [SerializeField] private string m_ResourcesPath = "";
    [SerializeField] private NumberTowerGameManager m_NumberTowerGameManager;
#if UNITY_EDITOR
    //[MenuItem("Tool/ReadCSV")]
    [Button("SaveCsv")]
    private void SaveCsvFile(){
        StartCoroutine(GetCsvFromGoogle());
    }
    [Button("ReadCsv")]
    private void ReadCsvFile(){

        Dictionary<int,NumberTowerRoomData> allRooms = new Dictionary<int, NumberTowerRoomData>(); 

        // room
        if(Resources.Load<TextAsset>("Room") == null){
            Debug.LogError(m_ResourcesPath+"/Room.csv is null" );
            return;
        }
        string json = Resources.Load<TextAsset>("Room").ToString();

        var contents = json.Split('\n',',');
        for (int i = 5; i < contents.Length; i=i+5)
        {
            NumberTowerRoomData room = new NumberTowerRoomData();
            room.Number = int.Parse(contents[i+1]);
            Color bgColor = Color.black;
            ColorUtility.TryParseHtmlString(contents[i+2], out bgColor);
            room.BGColor = bgColor;
            room.Operations = GetOperationByString(contents[i+3]);
            string test = contents[i+4].Trim();
            room.Prefab = GetNumberTowerPrefabByString(test);
            allRooms.Add(int.Parse(contents[i].Trim()),room);
        }

        // Level
        if(Resources.Load<TextAsset>("Level") == null){
            Debug.LogError(m_ResourcesPath+"/Level.csv is null" );
            return;
        }
        
        FileUtil.DeleteFileOrDirectory(m_LevelPath);
        AssetDatabase.Refresh();

        json = Resources.Load<TextAsset>("Level").ToString();
        contents = json.Split('\n',',');
            // level
            m_NumberTowerGameManager.ClearAllLevel();
            for (int i = 4; i < contents.Length; i=i+4)
            {
                //Debug.Log(contents[i]);
                LevelScriptable levelScriptable = ScriptableObject.CreateInstance<LevelScriptable>();
                string path = m_LevelPath+"/LV"+ (i/4f).ToString() +".asset";

                levelScriptable.PlayerHp = float.Parse(contents[i+1]);

                var allRoomId = contents[i+3].Split('|');
                var allColumeRoomCount = contents[i+2].Split('|');

                int index = 0;
                // create room for each colume
                foreach (var roomCount in allColumeRoomCount)
                {
                    NumberTowerColume numberTowerColume = new NumberTowerColume();
                    
                    for (int j = 0; j < int.Parse(roomCount); j++)
                    {

                        numberTowerColume.Rooms.Add(allRooms[int.Parse(allRoomId[index].Trim())]);
                        index++;
                    }

                    levelScriptable.Columes.Add(numberTowerColume);
                }

                AssetDatabase.CreateAsset(levelScriptable, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                m_NumberTowerGameManager.AddLevelScriptable(levelScriptable);
            }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }





    private IEnumerator GetCsvFromGoogle(){

        // Room
        UnityWebRequest www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vRfQPFboJJNlJ46EeqdMaTNR9GwYHOu5xiuIHuRm6dWsVvSQ10zE0c7ixeqxS857Y59TJ_OPNu_xymB/pub?gid=719668107&single=true&output=csv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            
            FileUtil.DeleteFileOrDirectory(m_ResourcesPath+"/Room.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            string json = www.downloadHandler.text;
            File.AppendAllText(m_ResourcesPath+"/Room.csv",json);

            
        }
        
        // level
        www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vRfQPFboJJNlJ46EeqdMaTNR9GwYHOu5xiuIHuRm6dWsVvSQ10zE0c7ixeqxS857Y59TJ_OPNu_xymB/pub?output=csv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            FileUtil.DeleteFileOrDirectory(m_ResourcesPath+"/Level.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            string json = www.downloadHandler.text;
            File.AppendAllText(m_ResourcesPath+"/Level.csv",json);

        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif

    private NumberTowerPrefab GetNumberTowerPrefabByString(string prefabString){
        switch (prefabString)
        {
            case "WhitePosion":
                return NumberTowerPrefab.WhitePosion;
            case "PurplePosion":
                return NumberTowerPrefab.PurplePosion;
            case "Death":
                return NumberTowerPrefab.Death;
            default:
                Debug.Log( "Undefined Prefab \""+prefabString+"\" . Use WhitePosion instead" );
                return NumberTowerPrefab.WhitePosion;
        }
    }

    private NumberTowerOperations GetOperationByString(string operationString){
        switch (operationString)
        {
            case "Enemy":
                return NumberTowerOperations.Enemy;
            case "Add":
                return NumberTowerOperations.Add;
            case "Minus":
                return NumberTowerOperations.Minus;
            case "Multiply":
                return NumberTowerOperations.Multiply;
            case "Divide":
                return NumberTowerOperations.Divide;
            default:
                Debug.Log( "Undefined Operation "+operationString+" . Use Add operation instead" );
                return NumberTowerOperations.Add;
        }

    }
}
