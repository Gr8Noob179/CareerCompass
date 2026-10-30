using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CareerCompass.Editor
{
    public static class WebGLBuilder
    {
        [MenuItem("Career Compass/Build WebGL")]
        public static void Build()
        {
            PlayerSettings.productName = "Career Compass";
            PlayerSettings.bundleVersion = "1.0." + DateTime.UtcNow.ToString("yyyyMMddHHmm");
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.WebGL.template = "PROJECT:CareerCompass";
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.dataCaching = false;
            PlayerSettings.WebGL.nameFilesAsHashes = true;

            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes = new string[] { "Assets/Scenes/Main.unity" };
            options.locationPathName = "Builds/WebGL";
            options.target = BuildTarget.WebGL;
            BuildReport report = BuildPipeline.BuildPlayer(options);

            if (report.summary.result == BuildResult.Succeeded)
            {
                GenerateServiceWorker();
            }

            Debug.Log("WebGL build " + report.summary.result + ", " + (report.summary.totalSize / 1048576f).ToString("0.0") + " MB");

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
            }
        }

        private static void GenerateServiceWorker()
        {
            bool isWindows = Application.platform == RuntimePlatform.WindowsEditor;
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(isWindows ? "cmd.exe" : "npm", isWindows ? "/c npm run service-worker" : "run service-worker");
            startInfo.WorkingDirectory = Path.GetFullPath("Web");
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new BuildFailedException("Workbox could not generate the service worker. Run npm install in the Web folder.\n" + output);
                }

                Debug.Log(output);
            }
        }
    }
}
