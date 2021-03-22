using FileManager.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace FileManager
{
    class Program
    {
        static string currentDirectory;
        static int rowsToDisplay = 18;
        static int recursionLevel = 2;
        static Square treeArea;
        static Square infoArea;
        static Square consoleArea;
        static bool IsArgsCountEnough(string[] command, int count)
        {
            if ((command.Length - 1) >= count)
            {
                return true;
            }
            else
            {
                infoArea.DisplayLine($"Comand {command[0]} needs {count} arguments!");
                return false;
            }
        }
        static bool AskUser(string question)
        {
            infoArea.DisplayLine($"{question} (введите: y / n)");
            if (consoleArea.Input("Введите ответ: ") == "y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Method for parse a comand from user.
        /// </summary>
        /// <param name="stringCommand">An input string which is need to parse.</param>
        static Commands ParseCommand(string stringCommand)
        {
            Commands userCommand;
            if (stringCommand == "")
            {
                userCommand = Commands.Unknown;
                infoArea.DisplayLine("команда то пустая");
            }
            else
            {
                string[] command = stringCommand.Split(' ');
                switch (command[0])
                {
                    case "cd":
                        userCommand = Commands.CurrentDirectory;
                        if(IsArgsCountEnough(command, 1))
                        {
                            ChangeCurrentDirectory(command[1]);
                        }
                        break;
                    case "cp":
                    case "copy":
                        userCommand = Commands.Copy;
                        if (IsArgsCountEnough(command, 2))
                        {
                            Copy(command[1], command[2], true);
                        }
                        break;
                    case "mv":
                    case "move":
                        userCommand = Commands.Move;
                        if (IsArgsCountEnough(command, 2))
                        {
                            Move(command[1], command[2], true);
                        }
                        break;
                    case "rm":
                    case "remove":
                        userCommand = Commands.Delete;
                        if (IsArgsCountEnough(command, 1))
                        {
                            Delete(command[1], true);
                        }
                        break;
                    case "ex":
                    case "exit":
                        userCommand = Commands.Exit;
                        break;
                    case "re":
                    case "refresh":
                        userCommand = Commands.Refresh;
                        break;
                    default:
                        userCommand = Commands.Unknown;
                        infoArea.DisplayLine("Неизвестная команда");
                        break;
                }
            }
            return userCommand;
        }
        /// <summary>
        /// This method check she new directory string and change current directory.
        /// </summary>
        /// <param name="newDirectory">String of new directory.</param>
        static List<Entry> GetEntriesList(string dir)
        {
            List<Entry> entriesList = new List<Entry>();
            string[] entries = Directory.GetFileSystemEntries(dir);
            foreach (string entry in entries)
            {
                entriesList.Add(new Entry(entry));
            }
            entriesList.Sort();
            return entriesList;
        }
        static void ShowDirectory(List<Entry> entireList, int recursionCount)
        {
            int spaceCount = recursionLevel - recursionCount;
            foreach (Entry entry in entireList)
            {
                Console.Write(new string(' ', spaceCount * 3));
                if (entry.Type == EntriesType.Directory)
                {
                    Console.WriteLine($"[ {entry.Name} ]");
                }
                else
                {
                    Console.WriteLine($"{entry.Name}");
                }
                if (entry.Type == EntriesType.Directory && recursionCount > 1)
                {
                    ShowDirectory(GetEntriesList(entry.Path), recursionCount - 1);
                }
            }
        }
        static void ChangeCurrentDirectory(string newDirectory)
        {
            newDirectory = MakeAbsolutePath(newDirectory);
            //Console.WriteLine();
            if (Directory.Exists(newDirectory))
            {
                currentDirectory = newDirectory;
                infoArea.DisplayLine($"Текущая директория применена: {newDirectory}");
            }
            else
            {
                infoArea.DisplayLine("Нет такой директории");
            }
        }
        static bool IsStringDir(string str)
        {
            Regex regex = new Regex(@"^(\w):\\.*");
            if (regex.IsMatch(str))
            {
                return true;
            }
            return false;
        }
        static string TrimStartingDirectorySeparator(string path)
        {
            Regex regex = new Regex(@"^\\.*");
            if (regex.IsMatch(path))
            {
                return path.Remove(0, 1);
            }
            return path;
        }
        static string MakeAbsolutePath(string path)
        {
            if (!IsStringDir(path))
            {
                path = TrimStartingDirectorySeparator(path);
                path = Path.Combine(currentDirectory, path);
            }
            return path;
        }
        /// <summary>
        /// Method for copy source to target.
        /// </summary>
        /// <param name="source">Can be a name of file in CD, path relative to CD or full path.</param>
        /// <param name="target">Can be path relative to CD or full path</param>
        static string MakeCopyName(string path)
        {
            //Regex regex = new Regex(@"_copy\((\d)+\)$");
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            //if (regex.IsMatch(name))
            //{
            //    name = "";
            //}
            return Path.Combine(dir, name + "_copy" + ext);
        }
        static void Copy(string source, string target, bool userInteraction)
        {
            source = MakeAbsolutePath(source);
            target = MakeAbsolutePath(target);
            //Если source файл
            if (File.Exists(source))
            {
                if (Directory.Exists(target))
                {
                    target = Path.Combine(target, Path.GetFileName(source));
                    if (source == target)
                    {
                        target = MakeCopyName(target);
                    }
                    if (File.Exists(target))
                    {
                        if (userInteraction)
                        {
                            if (AskUser($"Файл {target} уже существует. Заменить?"))
                            {
                                File.Copy(source, target, true);
                                infoArea.DisplayLine("Файл скопирован с заменой.");
                            }
                            else
                            {
                                infoArea.DisplayLine("Файл не скопирован.");
                            }
                        }
                        else
                        {
                            File.Copy(source, target, false);
                        }
                    }
                    else
                    {
                        File.Copy(source, target);
                        if (userInteraction)
                        {
                            infoArea.DisplayLine("Файл скопирован.");
                        }
                    }
                }
                else
                {
                    infoArea.DisplayLine("Несуществует пути target");
                }
            }
            //Если source папка
            else if (Directory.Exists(source))
            {
                if (Directory.Exists(target))
                {
                    //Начало логики копирования папки
                    target = Path.Combine(target, Path.GetFileName(source));
                    if (source == target)
                    {
                        target = MakeCopyName(target);
                    }
                    if (Directory.Exists(target))
                    {
                        if (userInteraction)
                        {
                            if (AskUser("Такая папка уже существует. Заменить?"))
                            {
                                Delete(target, false);
                                CopyFolder(source, target);
                                if (userInteraction)
                                {
                                    infoArea.DisplayLine("Папка скопирована с заменой");
                                }
                            }
                            else
                            {
                                infoArea.DisplayLine("Папка не скопирована!");
                            }
                        }
                        else
                        {
                            CopyFolder(source, target);
                            if (userInteraction)
                            {
                                infoArea.DisplayLine("Папка скопирована");
                            }
                        }
                    }
                    else
                    {
                        CopyFolder(source, target);
                        if (userInteraction)
                        {
                            infoArea.DisplayLine("Папка скопирована");
                        }
                    } 
                }
                // это если указали неверный путь куда надо скопироваь
                else
                {
                    infoArea.DisplayLine($"Директории {target} не существует");
                }
            }
            else
            {
                infoArea.DisplayLine("Неверно задан параметр source");
            }
        }
        static void CopyFolder(string source, string target)
        {
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
            string[] entries = Directory.GetFileSystemEntries(source);
            foreach (string entry in entries)
            {
                Copy(entry, target, false);
            }
        }
        static void Move(string source, string target, bool userInteraction)
        {
            source = MakeAbsolutePath(source);
            target = MakeAbsolutePath(target);
            // Если source файл
            if (File.Exists(source))
            {
                if (Directory.Exists(target))
                {
                    target = Path.Combine(target, Path.GetFileName(source));
                    if (source == target)
                    {
                        infoArea.DisplayLine("Source и Target совпадают. Действие невыполнено");
                    }
                    else
                    {
                        if (File.Exists(target))
                        {
                            if (userInteraction)
                            {
                                if (AskUser($"Файл {target} уже существует. Заменить?"))
                                {
                                    File.Move(source, target, true);
                                    infoArea.DisplayLine("Файл перемещен с заменой.");
                                }
                                else
                                {
                                    infoArea.DisplayLine("Файл не перемещен.");
                                }
                            }
                            else
                            {
                                File.Move(source, target, true);
                            }
                        }
                        else
                        {
                            File.Move(source, target);
                            if (userInteraction)
                            {
                                infoArea.DisplayLine("Файл перемещен.");
                            }
                        }
                    }
                }
                else
                {
                    infoArea.DisplayLine("Не существует пути target");
                }
            }
            // Если source папка
            else if (Directory.Exists(source))
            {
                if (Directory.Exists(target))
                {
                    target = Path.Combine(target, Path.GetFileName(source));
                    if (source == target)
                    {
                        infoArea.DisplayLine("Source и Target совпадают. Действие невыполнено");
                    }
                    else
                    {
                        if (Directory.Exists(target))
                        {
                            if (userInteraction)
                            {
                                if (AskUser("Такая папка уже существует. Заменить?"))
                                {
                                    Delete(target, false);
                                    MoveFolder(source, target);
                                }
                                else
                                {
                                    infoArea.DisplayLine("Папка не перемещена!");
                                }
                            }
                            else
                            {
                                MoveFolder(source, target);
                            }
                        }
                        else
                        {
                            MoveFolder(source, target);
                        }
                    }
                }
                // это если указали неверный путь куда надо скопироваь
                else
                {
                    infoArea.DisplayLine($"Директории {target} не существует");
                }
            }
            // Если source некорректный
            else
            {
                infoArea.DisplayLine("Неверно задан параметр source");
            }
        }
        static void MoveFolder(string source, string target)
        {
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
            string[] entries = Directory.GetFileSystemEntries(source);
            foreach (string entry in entries)
            {
                Move(entry, target, false);
            }
            Directory.Delete(source);
        }
        static void Delete(string path, bool userInteraction)
        {
            path = MakeAbsolutePath(path);
            // Если это файл
            if (File.Exists(path))
            {
                if (userInteraction)
                {
                    if (AskUser("Вы уверены, что хотите удалить файл?"))
                    {
                        File.Delete(path);
                    }
                }
                else
                {
                    File.Delete(path);
                    if (userInteraction)
                    {
                        infoArea.DisplayLine("Файл удален");
                    }
                }
            }
            // Если это папка
            else if (Directory.Exists(path))
            {
                if (userInteraction)
                {
                    if (AskUser("Вы уверены, что хотите удалить папку?"))
                    {
                        DeleteFolder(path);
                        if (userInteraction)
                        {
                            infoArea.DisplayLine("Папка удалена");
                        }
                    }
                }
                else
                {
                    DeleteFolder(path);
                    if (userInteraction)
                    {
                        infoArea.DisplayLine("Папка удалена");
                    }
                }
            }
            else
            {
                infoArea.DisplayLine("Bad target");
            }
        }
        static void DeleteFolder(string path)
        {
            string[] entries = Directory.GetFileSystemEntries(path);
            foreach (string item in entries)
            {
                Delete(item, false);
            }
            Directory.Delete(path);
        }
        static void Main(string[] args)
        {
            // Чтение настроек
            //rowsToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings.Get("rowsDisplay"));
            //recursionLevel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("recursionLevel"));

            // UI
            treeArea = new Square(0, 0, 120, rowsToDisplay + 2);
            treeArea.bottomLeftCornerSymbol = '╠';
            treeArea.bottomRightCornerSymbol = '╣';
            treeArea.MakeAllBorders();
            treeArea.Draw();
            infoArea = new Square(0, treeArea.posBorderBottom, 120, 7);
            infoArea.topLeftCornerSymbol = '╠';
            infoArea.topRightCornerSymbol = '╣';
            infoArea.bottomLeftCornerSymbol = '╠';
            infoArea.bottomRightCornerSymbol = '╣';
            infoArea.MakeAllBorders();
            infoArea.Draw();
            consoleArea = new Square(0, infoArea.posBorderBottom, 120, 3);
            consoleArea.topLeftCornerSymbol = '╠';
            consoleArea.topRightCornerSymbol = '╣';
            consoleArea.MakeAllBorders();
            consoleArea.Draw();
            Console.WriteLine();

            ChangeCurrentDirectory("D:\\TEMP");
            Commands userCommand;
            do
            {
                //ShowDirectory(GetEntriesList(currentDirectory), recursionLevel);
                userCommand = ParseCommand(consoleArea.Input($"{currentDirectory}>"));
            } while (userCommand != Commands.Exit);
        }
    }
}
