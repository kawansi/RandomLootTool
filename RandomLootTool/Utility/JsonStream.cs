using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace RandomVisualizerWPF.Utility
{
    public class JsonStream
    {
        public dynamic JsonData { get; private set; }
        public dynamic this[string key] => JsonData?[key];

        private DateTime lastWriteTime;
        private bool isAlive;

        private string? savePath;

        public JsonStream()
        {
        }

        public void SetPath(string path)
        {
            savePath = path;
        }

        public bool TryRefresh(bool ignoreTimestamps)
        {
            //handle file timestamps and attempt to read
            bool modified = false;
            if (savePath != null && (NeedsRefresh() || ignoreTimestamps))
            {
                if (TryOpen(savePath, out StreamReader stream) && TryRead(stream))
                {
                    isAlive = true;
                    modified = true;
                }
                else if (isAlive)
                {
                    isAlive = false;
                    modified = true;
                }
            }
            return modified;
        }

        private bool TryRead(StreamReader stream)
        {
            try
            {
                //read all json file contents and deserialize into dynamic json
                string fileContents = stream.ReadToEnd();
                JsonData = JsonConvert.DeserializeObject(fileContents);

                return true;
            }
            catch
            {
                JsonData = null;
                return false;
            }
            finally
            {
                stream?.Close();
            }
        }

        private bool NeedsRefresh()
        {
            if (TryGetLastWriteTime(out DateTime latestWriteTime))
            {
                if (lastWriteTime != latestWriteTime)
                {
                    lastWriteTime = latestWriteTime;
                    return true;
                }
            }
            else
            {
                isAlive = false;
            }
            return false;
        }

        private bool TryGetLastWriteTime(out DateTime lastWriteTime)
        {
            lastWriteTime = default;
            if (File.Exists(savePath))
            {
                try
                {
                    lastWriteTime = File.GetLastWriteTime(savePath);
                    return true;
                }
                catch { }
            }
            return false;
        }

        private bool TryOpen(string path, out StreamReader reader)
        {
            reader = null;
            if (File.Exists(path))
            {
                try
                {
                    var stream = new FileStream(path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite | FileShare.Delete);

                    reader = new StreamReader(stream);
                    return true;
                }
                catch
                {
                    isAlive = false;
                    Close(reader);
                }
            }
            return false;
        }
        private void Close(StreamReader reader) => reader?.Close();

        public void PrintAllData()
        {
            Trace.WriteLine(JsonData.ToString());
        }
    }
}