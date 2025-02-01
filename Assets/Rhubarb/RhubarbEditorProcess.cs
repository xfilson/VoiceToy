#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NAudio.Wave;
using UnityEditor;
using UnityEngine;

namespace FriendlyMonster.RhubarbTimeline
{
    [Serializable]
    public class SimpleRhubarbKeyframeData
    {
        public int frame;
        public MouthShape phoneme;
    }

    [Serializable]
    public class SimpleRhubarbTrackData
    {
        [SerializeField]
        public List<SimpleRhubarbKeyframeData> keyframes;

        public static SimpleRhubarbTrackData ConvertBy(RhubarbTrack rhubarbTrack)
        {
            SimpleRhubarbTrackData simpleRhubarbTrackData = new SimpleRhubarbTrackData();
            simpleRhubarbTrackData.keyframes = new List<SimpleRhubarbKeyframeData>(rhubarbTrack.keyframes.Count);
            for (int i = 0; i < rhubarbTrack.keyframes.Count; i++)
            {
                var curkeyFrame = rhubarbTrack.keyframes[i];
                SimpleRhubarbKeyframeData simpleRhubarbKeyframeData = new SimpleRhubarbKeyframeData();
                simpleRhubarbKeyframeData.frame = curkeyFrame.frame;
                simpleRhubarbKeyframeData.phoneme = curkeyFrame.phoneme;
                simpleRhubarbTrackData.keyframes.Add(simpleRhubarbKeyframeData);
            }
            return simpleRhubarbTrackData;
        }
    }

    public static class RhubarbEditorProcess
    {
        public static bool IsValid(string rhubarbPath)
        {
#if UNITY_EDITOR_WIN
            return rhubarbPath.EndsWith("rhubarb.exe");
#endif

#if UNITY_EDITOR_OSX
        return rhubarbPath.EndsWith("rhubarb");
#endif
        }

        private static string FixPath(string path)
        {
#if UNITY_EDITOR_WIN
            return path.Replace("/", "\\");
#endif
#if UNITY_EDITOR_OSX
            return path.Replace("\\", "/").Replace(" ", "\\ ");
#endif
        }

        /// <summary>
        /// 将mp3转wav;
        /// </summary>
        /// <param name="mp3FilePath"></param>
        /// <param name="wavFilePath"></param>
        public static void ConvertMp3ToWav(string mp3FilePath, string wavFilePath)
        {
            string ffmpegPath = Application.dataPath + "/../tools/ffmpeg/mac/ffmpeg";
            string arguments = $"-i \"{mp3FilePath}\" \"{wavFilePath}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Conversion successful.");
                }
                else
                {
                    Console.WriteLine("Conversion failed.");
                }
            }
        }
        
        /// <summary>
        /// 创建一个简单的嘴型轨道;
        /// </summary>
        /// <param name="rhubarbPath"></param>
        /// <param name="audioPath"></param>
        /// <param name="savedPath"></param>
        /// <returns></returns>
        public static SimpleRhubarbTrackData CreatSimpleRhubarbTrack(string audioPath, string savedPath)
        {
            string rhubarbPath = Path.Combine(Application.dataPath+"/../tools/Rhubarb-Lip-Sync-1.13.0-macOS", "rhubarb");
            savedPath = FixPath(savedPath);
            RhubarbTrack rhubarbTrack = RhubarbEditorProcess.Auto(rhubarbPath, audioPath, null, false, false, false);
            var simpleRhubarbTrackData = SimpleRhubarbTrackData.ConvertBy(rhubarbTrack);
            string jsonStr = JsonUtility.ToJson(simpleRhubarbTrackData);
            if (!Directory.Exists(Path.GetDirectoryName(savedPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savedPath));
            }
            File.WriteAllText(savedPath, jsonStr);
            return simpleRhubarbTrackData;
        }

        public static SimpleRhubarbTrackData GetSimpleRhubarbTrack(string savedPath)
        {
            if (File.Exists(savedPath))
            {
                string jsonStr = File.ReadAllText(savedPath);
                return JsonUtility.FromJson<SimpleRhubarbTrackData>(jsonStr);
            }
            return null;
        }

        public static RhubarbTrack Auto(string rhubarbPath, string audioPath, string dialog, bool _isG = true, bool _isH = true, bool _isX = true)
        {
            rhubarbPath = FixPath(rhubarbPath);
            audioPath = FixPath(audioPath);
            string dialogPath = FixPath(Path.Combine(Directory.GetCurrentDirectory(), "Assets/dialog.txt"));

            RhubarbTrack rhubarbTrack = ScriptableObject.CreateInstance<RhubarbTrack>();
            rhubarbTrack.keyframes = new List<RhubarbKeyframe>();

            bool isDialog = !string.IsNullOrEmpty(dialog);
            string extendedMouthShapesArgument = ExtendedMouthShapeArgument(_isG, _isH, _isX);

            if (isDialog)
            {
                File.WriteAllText(dialogPath, dialog);
            }

            Process process = new Process();

#if UNITY_EDITOR_WIN
            process.StartInfo.FileName = rhubarbPath;
            string command = "";
            command += "\"" + audioPath + "\" ";
            command += isDialog ? "--dialogFile \"" + dialogPath + "\" " : "";
            command += "--extendedShapes " + extendedMouthShapesArgument;
            process.StartInfo.Arguments = command;
#endif

#if UNITY_EDITOR_OSX
            process.StartInfo.FileName = "/bin/bash";
            string command = rhubarbPath + " ";
            command += audioPath + " ";
            command += isDialog ? "--dialogFile " + dialogPath + " " : "";
            command += "--extendedShapes " + extendedMouthShapesArgument;
            process.StartInfo.Arguments = "-c \" " + command + " \"";
#endif

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                RhubarbKeyframe keyframe = new RhubarbKeyframe();
                keyframe.Deserialize(line);
                rhubarbTrack.keyframes.Add(keyframe);
            }

            if (isDialog)
            {
                File.Delete(dialogPath);
            }

            return rhubarbTrack;
        }

        private static string ExtendedMouthShapeArgument(bool _isG, bool _isH, bool _isX)
        {
            string arg = "";
            if (_isG)
            {
                arg += "G";
            }
            if (_isH)
            {
                arg += "H";
            }
            if (_isX)
            {
                arg += "X";
            }

#if UNITY_EDITOR_WIN
            return arg.Length > 0 ? arg : "\"\"";
#endif
#if UNITY_EDITOR_OSX
            return arg.Length > 0 ? arg : "\'\'";
#endif
        }
    }
}
#endif