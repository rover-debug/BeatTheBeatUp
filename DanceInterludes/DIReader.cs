using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DIReader : MonoBehaviour
{
    [SerializeField]
    GameObject[] prefabs;
    // Start is called before the first frame update
    void Start()
    {
        TextAsset source = Resources.Load<TextAsset>("di_objects");
        if (!source) return;
        CsvReader csv = new CsvReader(source.text, true);
        while(csv.Read())
        {
            if (csv.GetFieldOrEmpty("type").Length == 0)
                return;
            //Debug.Log("Getting type " + csv.GetFieldOrEmpty("type"));
            DIObject type = (DIObject)Enum.Parse(typeof(DIObject), csv.GetFieldOrEmpty("type"), true);
            Vector3 pos;
            pos.x = float.Parse(csv.GetFieldOrEmpty("posx"));
            pos.y = float.Parse(csv.GetFieldOrEmpty("posy"));
            pos.z = float.Parse(csv.GetFieldOrEmpty("posz"));
            Quaternion q;
            q.x = float.Parse(csv.GetFieldOrEmpty("rotx"));
            q.y = float.Parse(csv.GetFieldOrEmpty("roty"));
            q.z = float.Parse(csv.GetFieldOrEmpty("rotz"));
            q.w = float.Parse(csv.GetFieldOrEmpty("rotw"));
            //Debug.Log("Generating " + type.ToString() + " at " + pos + " with rot " + q);
            MakeDIObject(type, pos, q);
        }
    }

    public void MakeDIObject(DIObject type, Vector3 pos, Quaternion q)
    {
        GameObject g = Instantiate(prefabs[(int)type], pos, q);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum DIObject
{
    Cube,
    Sphere,
    Cylinder
}
