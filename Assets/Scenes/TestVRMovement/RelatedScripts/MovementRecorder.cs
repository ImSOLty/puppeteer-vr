using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DataList
{
    public List<TimePositionRotation> keyFrameDatas;

    public DataList()
    {
        keyFrameDatas = new();
    }

    public void AddTimePositionRotation(TimePositionRotation tpr)
    {
        keyFrameDatas.Add(tpr);
    }
}

public class MovementRecorder : MonoBehaviour
{
    int framesToRecord = 10000;
    public TransformsOnly approach1, approach2, approach3, approach4;
    public string resultExport = "Helpers/_AnimationsData/results/";
    int recorded = 0;
    DataList approach1List, approach2List, approach3List, approach4List;

    void Start()
    {
        Application.targetFrameRate = 100;
        approach1List = new DataList();
        approach2List = new DataList();
        approach3List = new DataList();
        approach4List = new DataList();
    }

    void Update()
    {
        if (recorded < framesToRecord)
        {
            approach1List.AddTimePositionRotation(approach1.GetTransformsAsFloat(recorded));
            approach2List.AddTimePositionRotation(approach2.GetTransformsAsFloat(recorded));
            approach3List.AddTimePositionRotation(approach3.GetTransformsAsFloat(recorded));
            approach4List.AddTimePositionRotation(approach4.GetTransformsAsFloat(recorded));
            recorded += 1;
            Debug.Log(recorded);
        }
        else
        {
            File.WriteAllText(Path.Combine(Application.dataPath, resultExport, "1.json"), JsonUtility.ToJson(approach1List));
            File.WriteAllText(Path.Combine(Application.dataPath, resultExport, "2.json"), JsonUtility.ToJson(approach2List));
            File.WriteAllText(Path.Combine(Application.dataPath, resultExport, "3.json"), JsonUtility.ToJson(approach3List));
            File.WriteAllText(Path.Combine(Application.dataPath, resultExport, "4.json"), JsonUtility.ToJson(approach4List));
            Debug.Log("Done!");
            Destroy(this);
        }

    }
}
