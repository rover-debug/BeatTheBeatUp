// // using System.Collections;
// // using System.Collections.Generic;
// // using UnityEngine;
// using UnityEditor.Build.Reporting;
// using UnityEditor;
// class BuildScript
// {
//      static void PerformBuild ()
//      {
//         BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
//         buildPlayerOptions.scenes = new[] { "Assets/Punching.unity" };
//         buildPlayerOptions.locationPathName = "WindowsBuild";
//         buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
//         buildPlayerOptions.options = BuildOptions.None;

//         BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
//         BuildSummary summary = report.summary;

//         // if (summary.result == BuildResult.Succeeded)
//         // {
//         //     //Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
//         // }

//         // if (summary.result == BuildResult.Failed)
//         // {
//         //     //Debug.Log("Build failed");
//         // }
//      }
// }

