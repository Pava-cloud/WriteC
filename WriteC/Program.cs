using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
namespace WriteC
{
    struct subPaths {public string chat, share, main; }
    internal class Program
    {
        static void Main(string[] args)
        {
            int lang;
            try
            {
                System.IO.StreamReader languageReader = new StreamReader(@"..\..\lang.txt");
                lang = int.Parse(languageReader.ReadLine());
                languageReader.Close();
            }
            catch (Exception)
            {
                System.IO.StreamWriter languageWriter = new StreamWriter(@"..\..\lang.txt");
                Console.WriteLine("Please select language | Bitte wählen Sie ihre Sprache:");
                Console.WriteLine("1.....Deutsch \n2.....English");
                lang = int.Parse(Console.ReadLine());
                languageWriter.WriteLine(lang);
                languageWriter.Close();
            }
            string name;
            try
            {
                System.IO.StreamReader nameReader = new StreamReader(@"..\..\name.txt");
                name = nameReader.ReadLine();
                nameReader.Close();
            }
            catch (Exception)
            {
                System.IO.StreamWriter nameWriter = new StreamWriter(@"..\..\name.txt");
                if (lang == 1)
                {
                    Console.WriteLine("Bitte geben Sie ihren Namen ein");
                }
                else if (lang == 2)
                {
                    Console.WriteLine("Please enter Name");
                }
                name = Console.ReadLine();
                nameWriter.WriteLine(name);
                nameWriter.Close();
            }
            subPaths filePath;
            string directory = "H:";
            string line, txt;
            #region Input
            while (true)
            {
                Console.Clear();
                filePath.main = directory + @"\chat\";
                filePath.chat = filePath.main + @"chat.txt";
                filePath.share = filePath.main + @"share\";
                #region Reader/Output
                try
                {
                    System.IO.StreamReader log = new System.IO.StreamReader(filePath.chat);
                    txt = log.ReadToEnd();
                    log.Close();
                    Console.WriteLine(txt);
                }
                catch (Exception)
                { 
                    System.IO.StreamWriter sw1 = new System.IO.StreamWriter(filePath.chat);
                    sw1.Close();
                }
                #endregion Reader/Output
                line = Console.ReadLine();
                #region Edit Line
                if (line.ToLower().IndexOf("/editline") == 0)
                {
                    int lineNumber = int.Parse(line.Remove(0, "/editline ".Length).Remove(1));
                    var lines = File.ReadAllLines(filePath.chat).ToList();
                    if (lines[lineNumber].IndexOf(name) == 0)
                    {
                        if (lineNumber < lines.Count)
                        {
                            lines[lineNumber] = name + ": " + line.Remove(0, line.IndexOf(' ', line.IndexOf(' ') + 1) + 1);
                            File.WriteAllLines(filePath.chat, lines);
                        }
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
                #region Change Server
                else if (line.ToLower().IndexOf("/changeaddress") == 0 || line.ToLower().IndexOf("/changedir") == 0)
                {
                    string addressLine = line.Remove(0, line.IndexOf(" "));
                    if (addressLine.Length == addressLine.LastIndexOf(@"\"))
                    {
                        directory = addressLine.Remove(addressLine.Length);
                    }
                    else
                    {
                        directory = addressLine;
                    }
                }
                #endregion Change Server
                #region Help
                else if (line.ToLower().IndexOf("/help") == 0)
                {
                    int padSpace = 50;
                    switch (lang)
                    {
                        case 1:
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Liste aller Befehle:");
                                Console.ForegroundColor= ConsoleColor.White;
                                Console.WriteLine("/uploadfile {FilePath}".PadRight(padSpace - 2) + "Lade eine Datei auf den Server" +
                                    "\r\n/downloadfile {FileName}; {DownloadPath}".PadRight(padSpace) + "Lade eine Datei vom Server herunter" +
                                    "\r\n/changeaddress {NeuerPfad}".PadRight(padSpace) + "Ändere den aktuellen Server" +
                                    "\r\n/clear".PadRight(padSpace) + "Leere den kompletten Chatverlauf" +
                                    "\r\n/help".PadRight(padSpace) + "Zeige diese Liste aller Befehle");
                                break;
                            }
                        case 2:
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("List of all Commands:");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("/uploadfile {FilePath}".PadRight(padSpace - 2) + "upload a file to the server" +
                                    "\r\n/downloadfile {FileName}; {DownloadPath}".PadRight(padSpace) + "download a file from the server" +
                                    "\r\n/changeaddress {NewPath}".PadRight(padSpace) + "change the current server" + 
                                    "\r\n/clear".PadRight(padSpace) + "clear the chatlog" +
                                    "\r\n/help".PadRight(padSpace) + "show this list of all commands");
                                break;
                            }
                    }
                    Console.ReadKey();
                }
                #endregion Help
                else File.AppendAllText(filePath.chat, "\n" + name + ": " + line);
            }
            #endregion Input
        }
    }
}
