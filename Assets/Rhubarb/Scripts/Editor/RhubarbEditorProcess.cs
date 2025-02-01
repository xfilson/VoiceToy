using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NAudio.Wave;
using UnityEditor;
using UnityEngine;

namespace FriendlyMonster.RhubarbTimeline
{
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
            using (var reader = new AudioFileReader(mp3FilePath))
            {
                WaveFileWriter.CreateWaveFile16(wavFilePath, reader);
            }
        }
        
        /// <summary>
        /// 创建一个简单的嘴型轨道;
        /// </summary>
        /// <param name="rhubarbPath"></param>
        /// <param name="audioPath"></param>
        /// <param name="savedPath"></param>
        /// <returns></returns>
        public static RhubarbTrack CreatSimpleRhubarbTrack(string rhubarbPath, string audioPath, string savedPath)
        {
            savedPath = FixPath(savedPath);
            RhubarbTrack rhubarbTrack = RhubarbEditorProcess.Auto(rhubarbPath, audioPath, null, false, false, false);
            AssetDatabase.CreateAsset(rhubarbTrack, savedPath); // 保存为Asset文件
            return rhubarbTrack;
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