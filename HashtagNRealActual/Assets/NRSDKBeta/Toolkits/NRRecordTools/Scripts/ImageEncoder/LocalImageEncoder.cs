using System.IO;
using UnityEngine;

namespace NRKernal.Record.Tool
{
    public class LocalImageEncoder : IImageEncoder
    {
        private string BasePath;

        public LocalImageEncoder()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            BasePath = System.IO.Directory.GetParent(Application.dataPath).ToString();
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            BasePath = "file://" + System.IO.Directory.GetCurrentDirectory();
#elif UNITY_ANDROID
            BasePath ="/storage/emulated/0/";
#endif
        }

        private string GetSavePath(FrameType frametype)
        {
            string path, folder;

            folder = "/RecordImages/" + frametype.ToString();
            path = BasePath + folder;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public void Commit(FrameData frame)
        {
            // For testing purposes, also write to a file in the project folder
            string path = string.Format(GetSavePath(frame.frameType) + "/{0}.png", frame.timeStamp);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream file = File.Create(path);
            file.Write(frame.imageData, 0, frame.imageData.Length);
            file.Flush();
            file.Close();

            Debug.Log("write a frame data :" + path);
        }

        public void Stop() { }
    }
}
