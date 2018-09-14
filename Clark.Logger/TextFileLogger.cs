using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clark.Logger
{
    public class TextFileLogger
    {
        private static readonly object _syncObject = new object();

        public static void Log(string path, string fileName, string log)
        {
            lock (_syncObject)
            {
                System.IO.Directory.CreateDirectory(path);

                string dir = Path.Combine(path, fileName);
                // This text is added only once to the file.
                if (!File.Exists(dir))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(dir))
                    {
                        sw.WriteLine(log);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(dir))
                    {
                        sw.WriteLine(log);
                    }
                }
            }
        }

        public static void WriteOverwriteFile(string path, string fileName, List<string> lines)
        {
            System.IO.Directory.CreateDirectory(path);

            string dir = Path.Combine(path, fileName);
            // This text is added only once to the file.
            File.Delete(dir);
            if (!File.Exists(dir))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(dir))
                {
                    foreach (string line in lines)
                        sw.WriteLine(line);
                }
            }
            //else
            //{
            //    using (StreamWriter sw = File.AppendText(dir))
            //    {
            //        sw.WriteLine(log);
            //    }
            //}
        }
    }
}