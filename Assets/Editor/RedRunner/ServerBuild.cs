﻿using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Diagnostics;

public class ServerBuild
{
	[MenuItem("Server/Build")]
	public static void BuildServer()
	{
		string path = EditorUtility.SaveFolderPanel("Choose Build Directory", "", "Build");

		var options = new BuildPlayerOptions
		{
			scenes = new string[] { "Assets/Scenes/Play.unity" },
			locationPathName = path + "/rr-server",
			targetGroup = BuildTargetGroup.Standalone,
			target = BuildTarget.StandaloneLinux64,
			options = BuildOptions.EnableHeadlessMode | BuildOptions.Development
		};

		var report = BuildPipeline.BuildPlayer(options);
		var summary = report.summary;

		if (summary.result == BuildResult.Succeeded)
		{
			Debug.Print("Build succeeded: " + summary.totalSize + " bytes");
		}

		if (summary.result == BuildResult.Failed)
		{
			Debug.Print("Build failed");
		}
	}
}
