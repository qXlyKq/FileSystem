using System;
using System.Security.AccessControl;

namespace FileSystem
{
    class Program
    {
        public static void Main()
        {

            String startPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            while (true)
            {
                 var res = Console.ReadLine();
                 var changedPass = FindCommand(res,startPath);
                 if ( changedPass == "stop")
                 {
                    return;
                 }
                 startPath = changedPass;
            }    
        }

        public static string FindCommand(string cmd, string path)
        {
            string newPath = path;
            switch (cmd)
            {
                case string a when a.Contains("pwd"):
                    Console.WriteLine(path);
                    break;
                case string b when b.Contains("cd"):
                    newPath = CdCommand(cmd, path);
                    break;
                case string c when c.Contains("ls"):
                    var result = LsCommand(path);
                    break;
                case "e":
                    newPath = "stop";
                    break;
                case string d when d.Contains("touch"):
                    CreateNewFile(cmd,path);
                    break;
                case string g when g.Contains("cat"):
                    ReadFromFile(cmd, path);
                    break;
                case string b when b.Contains("insert"):
                    InsertTextIntoFile(cmd.Split(' ')[1], cmd.Split(' ')[2], cmd.Split(' ')[3], path);
                    break;
                case string o when o.Contains("csvCat"):
                    var filePath = $@"{path}\{cmd.Split(' ')[1]}";
                    bool isTable = false;
                    if (cmd.Split(' ').Length > 2)
                    {
                        if (cmd.Split(' ')[2] == "-t")
                        {
                            isTable = true;
                        }
                    }
                    DisplayFileContents(filePath, isTable);
                    break;
                default:
                    Console.WriteLine("you wrote wrong!");
                    break;
            }
            return newPath;
        }

        static void DisplayFileContents(string fileName, bool isTableFormat)
        {
            try
            {
                string fileExtension = Path.GetExtension(fileName).ToLower();

                if (fileExtension != ".csv")
                {
                    throw new ArgumentException("Unsupported file format. Only CSV files are supported for table display.");
                }

                string delimiter = ",";
                if (File.Exists(fileName))
                {
                    string fileContent;
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        fileContent = reader.ReadToEnd();
                    }

                    if (fileContent.Contains(";"))
                    {
                        delimiter = ";";
                    }

                    if (isTableFormat)
                    {
                        DisplayTable(fileContent, delimiter);
                    }
                    else
                    {
                        Console.WriteLine(fileContent);
                    }
                }
                else
                {
                    throw new FileNotFoundException($"File not found: {fileName}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to display file contents. {e.Message}");
            }
        }

        static void DisplayTable(string fileContent, string delimiter)
        {
            string[] lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                string[] columns = line.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var column in columns)
                {
                    Console.Write($"{column,-15}");
                }
                Console.WriteLine();
            }
        }

        static void InsertTextIntoFile(string fileName, string pos, string text,string path)
        {
            try
            {
                string fileContent;
                var filePath = $@"{path}\{fileName}";
                using (StreamReader reader = new StreamReader(filePath))
                {
                    fileContent = reader.ReadToEnd();
                }

                int position;
                if (int.TryParse(pos, out int intValue))
                {
                    position = intValue;

                    if (position < 0 || position > fileContent.Length)
                    {
                        throw new ArgumentOutOfRangeException("pos", "Invalid position value. Position should be within the file boundaries.");
                    }
                }
                else if (pos is string && ((string)pos).ToLower() == "end")
                {
                    position = fileContent.Length; // Вставка в кінець файлу
                }
                else if (pos is string && ((string)pos).ToLower() == "start")
                {
                    position = 0; // Вставка в початок файлу
                }
                else
                {
                    throw new ArgumentException("Invalid value for pos parameter.");
                }

                fileContent = fileContent.Insert(position, text);

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(fileContent);
                    Console.WriteLine("You text has added into file");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to insert text into the file. {e.Message}");
            }
        }

        public static void ReadFromFile (string cmd, string path)
        {
            var filePath = $@"{path}\{cmd.Split(' ')[1]}";
            try
            { 
                using (StreamReader input = new StreamReader(filePath))
                {
                    int counter = 0;
                    string ln;

                    while ((ln = input.ReadLine()) != null)
                    {
                        Console.WriteLine(ln);
                        counter++;
                    }
                    input.Close();
                    Console.WriteLine($"File has {counter} lines.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void CreateNewFile(string cmd, string path)
        {
            var currentPath = @$"{path}\{cmd.Split(' ')[1]}" ;
            var newFile = File.Create(currentPath);
            if (File.Exists(currentPath))
            {
                Console.WriteLine($"File {cmd.Split(' ')[1]} is created");
            }
            else
            {
                Console.WriteLine("Some error");
            }
            return;
        }
        public static string LsCommand(string path)
        {
            string result = "";

            var dirs = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            foreach (var dir in dirs)
            {
                Console.WriteLine(dir);
            }
            Console.WriteLine("SomeFiles");
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                //fileInfo.GetAccessControl();
                Console.WriteLine($"{file} + {fileInfo.Length} bytes + {fileInfo.Attributes} + {fileInfo.LastWriteTime}");
            }

            return result;
        }

        public static string CdCommand(string cmd,string newPath)
        {
            var stringJoin = cmd.Split(' ');
            var path = stringJoin[1].Trim('/');
            newPath = newPath + path;
            return newPath;
            
        }
    }
}