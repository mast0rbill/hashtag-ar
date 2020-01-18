namespace NRKernal.Record.Tool
{
    using System.IO;
    using UnityEngine;

    public class FileStreamImageEncoder : IImageEncoder
    {
        private string BasePath;
        FileStream fileStream;

        public FileStreamImageEncoder()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            BasePath = System.IO.Directory.GetParent(Application.dataPath).ToString();
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            BasePath = "file://" + System.IO.Directory.GetCurrentDirectory();
#elif UNITY_ANDROID
            BasePath ="/storage/emulated/0/";
#endif
            string path = @"D:\WorkSpace\Projects\GitLab\NRSDKForUnity\RecordImages\test.png";
            if (!File.Exists(path))
            {
                using (File.Create(path))
                {
                }
            }
            fileStream = new FileStream(path, FileMode.Open);
        }

        ~FileStreamImageEncoder()
        {
            fileStream.Flush();
            fileStream.Close();
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
            Debug.Log("FileStreamImageEncoder start to encode to file:" + frame.imageData.Length);
            fileStream.Write(frame.imageData, 0, frame.imageData.Length);
        }

        public void Stop() { }
    }
}
