using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadTsv : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetCsvData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GetCsvData(){
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

}