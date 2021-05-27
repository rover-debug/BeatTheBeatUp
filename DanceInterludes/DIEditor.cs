using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DIEditor : MonoBehaviour
{
    #region Singleton
    public static DIEditor instance;

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("More than one instance of DIEditor found!");
            return;
        }

        instance = this;
    }
    #endregion

    enum CreateMode
    {
        Append,
        Overwrite
    }

    [SerializeField]
    GameObject rootPosReference;
    [SerializeField]
    float playerHeight;
    [SerializeField]
    CreateMode createMode = CreateMode.Append;
    [SerializeField]
    string outputFile = "di_objects.csv";

    List<GameObject> objects = new List<GameObject>();

    private bool orderMode;
    private int orderIdx = -1;
    List<GameObject> ordered = new List<GameObject>();

    public void MakeEditorObject(DIObject type, GameObject prefab, Transform origin)
    {
        GameObject newobj = Instantiate(prefab, origin.position, Quaternion.identity);
        newobj.AddComponent<EditorObject>();
        newobj.GetComponent<EditorObject>().type = type;
        objects.Add(newobj);
    }

    public bool IsOrderMode()
    {
        return orderMode;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            // TODO: move player into playerPose
        }

        if (!orderMode && OVRInput.GetDown(OVRInput.RawButton.X))
        {
            orderMode = true;
            foreach(GameObject obj in objects)
            {
                if (!obj.GetComponent<EditorObject>().IsUsed())
                {
                    obj.SetActive(false);
                }
            }
        }
        else if(orderMode && OVRInput.GetDown(OVRInput.RawButton.X))
        {

            orderMode = false;
        }
    }

    public int GetNextIndex(GameObject obj)
    {
        if(ordered.Count == 0 || obj != ordered[ordered.Count - 1])
        {
            ordered.Add(obj);
            ++orderIdx;
        }
        return orderIdx;
    }

    public void OutputObjects()
    {
        if(ordered.Count == 0)
        {
            return;
        }

        string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Resources", outputFile);

        if(createMode == CreateMode.Append && File.Exists(path))
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                OutputObjectsHelper(sw);
            }
        }
        else
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("type,posx,posy,posz,rotx,roty,rotz,rotw");
                OutputObjectsHelper(sw);
            }
        }
    }

    private void OutputObjectsHelper(StreamWriter sw)
    {
        foreach (GameObject obj in ordered)
        {
            Vector3 pos = obj.transform.position;
            Quaternion q = obj.transform.rotation;
            sw.WriteLine(obj.GetComponent<EditorObject>().type.ToString() + "," + pos.x + "," + pos.y + "," + pos.z + "," + q.x + "," + q.y + "," + q.z + "," + q.w);
        }
    }
}
