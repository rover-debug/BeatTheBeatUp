using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewObjectGenerator : MonoBehaviour
{
    public GameObject NewCube;
    public DIObject type;
    private void Awake()
    {
        
    }

    private void Start()
    {
        DIEditor.instance.MakeEditorObject(type, NewCube, transform);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == (NewCube.gameObject.name + "(Clone)"))
        {
            DIEditor.instance.MakeEditorObject(type, NewCube, transform);
            other.GetComponent<EditorObject>().SetUsed();
        }
    }
}
