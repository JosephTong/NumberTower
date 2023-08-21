using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using AdvancedEditorTools.Attributes;

public class ReadTsv : MonoBehaviour
{
#if UNITY_EDITOR
    //[MenuItem("Tool/ReadTSV")]
    [Button("ReadTsv")]
    private void ReadTsvFile(){
        StartCoroutine(GetTsvFromGoogle());
    }

    private IEnumerator GetTsvFromGoogle(){
        // room
        UnityWebRequest www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vRfQPFboJJNlJ46EeqdMaTNR9GwYHOu5xiuIHuRm6dWsVvSQ10zE0c7ixeqxS857Y59TJ_OPNu_xymB/pub?gid=719668107&single=true&output=tsv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            string json = www.downloadHandler.text;
            Debug.Log(json);
            var tmp = json.Split('\t');
/*
            foreach (var item in tmp)
            {
              Debug.Log(item);
            }*/
        }

        //level
        www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vRfQPFboJJNlJ46EeqdMaTNR9GwYHOu5xiuIHuRm6dWsVvSQ10zE0c7ixeqxS857Y59TJ_OPNu_xymB/pub?gid=0&single=true&output=tsv");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log("Error: " + www.error);
        }else{
            string json = www.downloadHandler.text;
            Debug.Log(json);
            var tmp = json.Split('\t');
        }
    }
#endif
}