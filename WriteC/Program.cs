using System;
using System.IO;
using System.Linq;
using static System.Console;
using static System.ConsoleColor;
namespace WriteC
{
    struct subPaths {public string chat, share, main; }
    internal class Program
    {
        static void Main(string[] args)
        {
            const int serverOS = 0; // 0 = Windows | 1 = Linux | 2 = Mac
            const int key = 12;
            int lang;
            bool darkModeToggle = true, newUser = false;
            string line, txt, name, directory = "";
            subPaths filePath;
            try
            {
                using (StreamReader pathreader = new StreamReader("path.txt"))
                {
                    directory = pathreader.ReadLine();
                }
            }
            catch (Exception)
            {
                StreamWriter pathwriter = new StreamWriter("path.txt");
#pragma warning disable CS0162 //Disable Warning message
                switch (serverOS)
                {
                    case 0: pathwriter.WriteLine("C:");
                        break;
                    case 1: pathwriter.WriteLine("/usr/home/$(whoami)");
                        break;
                    case 2: pathwriter.WriteLine("~");
                        break;
                }
                pathwriter.Close();
            }
            (directory, filePath) = Directory(directory, serverOS);
            #region Startup
            try
            {
                StreamReader languageReader = new StreamReader(@"..\..\lang.txt");
                lang = int.Parse(languageReader.ReadLine());
                languageReader.Close();
            }
            catch (Exception)
            {
                StreamWriter languageWriter = new StreamWriter(@"..\..\lang.txt");
                WriteLine("Please select language | Bitte wählen Sie ihre Sprache:");
                WriteLine("1.....Deutsch \n2.....English");
                lang = int.Parse(ReadLine());
                languageWriter.WriteLine(lang);
                languageWriter.Close();
            }
            try
            {
                StreamReader nameReader = new StreamReader(@"..\..\name.txt");
                name = nameReader.ReadLine();
                nameReader.Close();
            }
            catch (Exception)
            {
                StreamWriter nameWriter = new StreamWriter(@"..\..\name.txt");
                if (lang == 1)
                {
                    WriteLine("Bitte geben Sie ihren Namen ein");
                }
                else if (lang == 2)
                {
                    WriteLine("Please enter Name");
                }
                name = ReadLine();
                nameWriter.WriteLine(name);
                nameWriter.Close();
                newUser = true;
            }
            if (newUser)
            {
                File.AppendAllText(filePath.chat, "\n" + name + " joined the chat.");
                newUser = false;
            }
            #endregion Startup
            #region Input
            #region Welcome Message
            if (lang == 1)
            {
                WriteLine("Willkommen zu WriteC");
                WriteLine("Geben Sie /help fär eine liste aller gültigen Befehle ein:");
            }
            else if (lang == 2)
            {
                WriteLine("Welcome to WriteC");
                WriteLine("Enter /help to show a list of all commands");
            }
            #endregion Welcome Message
            ReadKey(true);
            while (true)
            {
                (directory, filePath) = Directory(directory, serverOS);
                Clear();
                #region Reader/Output
                try
                {
                    using (StreamReader log = new StreamReader(filePath.chat))
                    {
                        while ((txt = log.ReadLine()) != null)
                        {
                            WriteLine(Decoding(txt, key));
                        }
                        txt = log.ReadToEnd();
                    }
                }
                catch (Exception)
                { 
                    StreamWriter sw1 = new StreamWriter(filePath.chat);
                    sw1.Close();
                }
                #endregion Reader/Output
                line = ReadLine();
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
                    File.AppendAllText(filePath.chat, "\n" + name + " just uploaded a file: " + line.Remove(0, line.LastIndexOf(@"\") + 1));
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
                    StreamWriter clearFile = new StreamWriter(filePath.chat);
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
                    const int padSpace = 50;
                    switch (lang)
                    {
                        case 1:
                            {
                                ForegroundColor = Yellow;
                                WriteLine("Liste aller Befehle:");
                                ForegroundColor = White;
                                WriteLine("/uploadfile {FilePath}".PadRight(padSpace - 2) + "Lade eine Datei auf den Server" +
                                    "\r\n/downloadfile {FileName}; {DownloadPath}".PadRight(padSpace) + "Lade eine Datei vom Server herunter" +
                                    "\r\n/changeaddress {NeuerPfad}".PadRight(padSpace) + "Ändere den aktuellen Server" +
                                    "\r\n/foreground-color {Farbe}".PadRight(padSpace) + "Ändere die Textfarbe" +
                                    "\r\n/background-color {Farbe}".PadRight(padSpace) + "Ändere die Hintergrundfarbe" +
                                    "\r\n/darkmode".PadRight(padSpace) + "Wechsle in den Darkmode" +
                                    "\r\n/clear".PadRight(padSpace) + "Leere den kompletten Chatverlauf" +
                                    "\r\n/help".PadRight(padSpace) + "Zeige diese Liste aller Befehle"
                                    );
                                break;
                            }
                        case 2:
                            {
                                ForegroundColor = Yellow;
                                WriteLine("List of all Commands:");
                                ForegroundColor = White;
                                WriteLine("/uploadfile {FilePath}".PadRight(padSpace - 2) + "upload a file to the server" +
                                    "\r\n/downloadfile {FileName}; {DownloadPath}".PadRight(padSpace) + "download a file from the server" +
                                    "\r\n/changeaddress {NewPath}".PadRight(padSpace) + "change the current server" +
                                    "\r\n/foreground-color {Color}".PadRight(padSpace) + "change the text color" +
                                    "\r\n/background-color {Color}".PadRight(padSpace) + "change the background color" +
                                    "\r\n/darkmode".PadRight(padSpace) + "change to darkmode" +
                                    "\r\n/clear".PadRight(padSpace) + "clear the chatlog" +
                                    "\r\n/help".PadRight(padSpace) + "show this list of all commands"
                                    );
                                break;
                            }
                    }
                    ReadKey();
                }
                #endregion Help
                #region Foreground Color
                else if (line.ToLower().IndexOf("/foregroundcolor") == 0 || line.ToLower().IndexOf("/foreground-color") == 0)
                {
                    string colorName = line.Remove(0, "/foregroundcolor".Length + 1);
                    ConsoleColor color;
                    if (Enum.TryParse(colorName, true, out color))
                    {
                        ForegroundColor = color;
                        WriteLine("This text is red!");
                    }
                    else
                    {
                        WriteLine("Invalid color name.");
                    }
                }
                #endregion Foreground Color
                #region Background Color
                else if (line.ToLower().IndexOf("/backgroundcolor") == 0 || line.ToLower().IndexOf("/background-color") == 0)
                {
                    string colorName = line.Remove(0, line.IndexOf(' '));
                    ConsoleColor color;
                    if (Enum.TryParse(colorName, true, out color))
                    {
                        BackgroundColor = color;
                        WriteLine("This text is red!");
                    }
                    else
                    {
                        WriteLine("Invalid color name.");
                    }
                }
                #endregion Foreground Color
                #region Darkmode
                else if (line.IndexOf("/darkmode") == 0)
                {
                    if (darkModeToggle)
                    {
                        ForegroundColor = ConsoleColor.Black;
                        BackgroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.White;
                        BackgroundColor = ConsoleColor.Black;
                    }
                    darkModeToggle = !darkModeToggle;
                }
                #endregion Darkmode
                #region Write Line
                else
                {
                    line = "\n" + name + ": " + line;
                    char[] currentChar = new char[line.Length];
                    File.AppendAllText(filePath.chat, Encoding(line, key));
                }
                #endregion Write Line
            }
            #endregion Input
        }
        public static string Encoding(string message, int key)
        {
            string newMessage = "";
            WriteLine(message);
            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                int code = char.Parse(message.Substring(i, 1));
                newMessage += ((char)(code + key)).ToString();
            }
            return newMessage;
        }

        public static string Decoding(string message, int key)
        {
            string newMessage = "";
            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                int code = char.Parse(message.Substring(i, 1));
                newMessage += ((char)(code - key)).ToString();
            }
            return newMessage;
        }
        public static (string, subPaths) Directory(string directory, int serverOS)
        {
            subPaths filePath;
            switch (serverOS)
            {
                default:
                    filePath.main = directory + @"\chat\";
                    filePath.chat = filePath.main + @"chat.txt";
                    filePath.share = filePath.main + @"share\";
                    break;
                case 1:
                    filePath.main = directory + @"/chat/";
                    filePath.chat = filePath.main + @"chat.txt";
                    filePath.share = filePath.main + @"share/";
                    break;
                case 2:
                    filePath.main = directory + @"/chat/";
                    filePath.chat = filePath.main + @"chat.txt";
                    filePath.share = filePath.main + @"share/";
                    break;
            }
            return (directory, filePath);
        }
    }
}
