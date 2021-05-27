using UnityEngine;
using UnityEditor;

// Ensure class initializer is called whenever scripts recompile
// [InitializeOnLoad]
// public class OnPlayModeQuit : MonoBehaviour
// {
//     [System.Obsolete]
//     private void Start()
//     {
//         EditorApplication.playmodeStateChanged += ModeChanged;
//     }

//     static void ModeChanged()
//     {
//         if (!EditorApplication.isPlayingOrWillChangePlaymode &&
//              EditorApplication.isPlaying)
//         {
//             DIEditor.instance.OutputObjects();
//         }
//     }
// }

