using System;
using System.IO;
using System.Linq;
namespace WriteC
{
    struct subPaths {public string chat, share, main; }
    internal class Program
    {
        static void Main(string[] args)
        {
            const char driveName = 'H';
            const string name = "Name";

            subPaths filePath;
            filePath.main = driveName + @":\chat\";
            filePath.chat = filePath.main + @"chat.txt";
            filePath.share = filePath.main + @"share\";
            string line, txt;
            #region Input
            while (true)
            {
                #region Reader/Output
                try
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(filePath.chat);
                    txt = sr.ReadToEnd();
                    sr.Close();
                    Console.WriteLine(txt);
                }
                catch (Exception)
                { 
                    System.IO.StreamWriter sw1 = new System.IO.StreamWriter(filePath.chat);
                    System.IO.StreamWriter sw2 = new System.IO.StreamWriter(filePath.share);
                }
                #endregion Reader/Output
                line = Console.ReadLine();
                #region Edit Line
                if (line.ToLower().IndexOf("/editline") == 0)
                {
                    int lineNumber = int.Parse(line.Remove(0, "/editline ".Length).Remove(1));
                    var lines = File.ReadAllLines(filePath.chat).ToList();
                    if (lineNumber < lines.Count)
                    {
                        lines[lineNumber] = name + ": " + line.Remove(0, line.IndexOf(' ', line.IndexOf(' ') + 1) + 1);
                        File.WriteAllLines(filePath.chat, lines);
                    }
                }
                #endregion Edit Line
                #region File Upload
                else if (line.ToLower().IndexOf("/uploadfile") == 0)
                {
                    File.Move(line.Remove(0, "/uploadfile ".Length), filePath.share + line.Remove(0, line.LastIndexOf(@"\")));
                    File.AppendAllText(filePath.chat,"\n" + name + " just uploaded a file: " + line.Remove(0, line.LastIndexOf(@"\") + 1));
                }
                #endregion File Upload
                #region File Download
                else if (line.ToLower().IndexOf("/downloadfile ") == 0)
                {
                    string commandLine = line.Remove(0, "/downloadfile".Length + 1);
                    string[] parts = commandLine.Split(';');
                    File.Move(filePath.share + parts[0], parts[1].Remove(0, 1));
                }
                #endregion File Download
                #region Clear
                else if (line.ToLower().IndexOf("/clear") == 0)
                {
                    System.IO.StreamWriter clearFile = new System.IO.StreamWriter(filePath.chat);
                    clearFile.Close();
                }
                #endregion Clear
                else File.AppendAllText(filePath.chat, "\n" + name + ": " + line);
                Console.Clear();
            }
            #endregion Input
        }
    }
}
