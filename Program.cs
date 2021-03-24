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
        static int rowsToDisplay;
        static int recursionLevel;
        static string defaultDir = "C:\\";
        static Square treeArea;
        static Square infoArea;
        static Square consoleArea;
        static List<string> displayList = new List<string>();
        static List<string>[] displayPages;
        static int currentDisplayPage = 0;
        /// <summary>
        /// Проверяет достаточно ли аргументов в переданной строке.
        /// </summary>
        /// <param name="command">Массив строк, содержажий команду первым элементой.</param>
        /// <param name="count">Количество необходимых аргументов для команды.</param>
        /// <returns>true если аргументов достаточно, false если не достаточно.</returns>
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
        /// <summary>
        /// Выводит в панель информации вопрос, затем анализирует ответ пользователя, введенный в консоль.
        /// </summary>
        /// <param name="question">Вопрос, который нужно вывести в инфопанель.</param>
        /// <returns>true если ответ пользователя y, в остальных случаях false.</returns>
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
        /// Разбирает строковую команду по эелементо с разделителем "пробел", и выполняет необходимый метод, соответствующей команде.
        /// </summary>
        /// <param name="stringCommand">An input string which is need to parse.</param>
        /// <returns>true если была введена команда exit, в остальных случаях false.</returns>
        static bool ParseCommand(string stringCommand)
        {
            bool isCommandExit = false;
            if (stringCommand == "")
            {
                infoArea.DisplayLine("Вы не ввели команду!");
            }
            else
            {
                string[] command = stringCommand.Split(' ');
                switch (command[0])
                {
                    case "cd":
                        if(IsArgsCountEnough(command, 1))
                        {
                            ChangeCurrentDirectory(command[1], true);
                        }
                        break;
                    case "cp":
                    case "copy":
                        if (IsArgsCountEnough(command, 2))
                        {
                            Copy(command[1], command[2], true);
                        }
                        break;
                    case "mv":
                    case "move":
                        if (IsArgsCountEnough(command, 2))
                        {
                            Move(command[1], command[2], true);
                        }
                        break;
                    case "rm":
                    case "remove":
                        if (IsArgsCountEnough(command, 1))
                        {
                            Delete(command[1], true);
                        }
                        break;
                    case "ex":
                    case "exit":
                        isCommandExit = true;
                        break;
                    case "re":
                    case "refresh":
                        break;
                    case ">":
                        NextPage();
                        break;
                    case "<":
                        PrevPage();
                        break;
                    case "help":
                        ShowHelp();
                        break;
                    case "info":
                        if (IsArgsCountEnough(command, 1))
                        {
                            ShowInfo(command[1]);
                        }
                        break;
                    default:
                        infoArea.DisplayLine("Команда не распознана!");
                        break;
                }
            }
            return isCommandExit;
        }
        /// <summary>
        /// Метод для получения списка элементов в директории.
        /// </summary>
        /// <param name="dir">Пусть к папке.</param>
        /// <returns>Возвращает список элементов.</returns>
        static List<Entry> GetEntriesList(string dir)
        {
            List<Entry> entriesList = new List<Entry>();
            try
            {
                string[] entries = Directory.GetFileSystemEntries(dir);
                foreach (string entry in entries)
                {
                    entriesList.Add(new Entry(entry));
                }
                entriesList.Sort();
            }
            catch (Exception e)
            {
                LogException(e);
            }
            
            return entriesList;
        }
        /// <summary>
        /// Рекурсивно создаёт список строк необходимых вывести на косоль.
        /// </summary>
        /// <param name="entireList">Список элементов какой либо директории.</param>
        /// <param name="recursionCount">Необходимая глубина отображения исходной папки.</param>
        static void MakeDisplayList(List<Entry> entireList, int recursionCount)
        {
            string displayString;
            int spaceCount = recursionLevel - recursionCount;
            if (spaceCount == 0)
            {
                displayString = "[ .. ]";
                displayList.Add(displayString);
            }
            foreach (Entry entry in entireList)
            {
                displayString = "";
                displayString += " " + new string(' ', spaceCount * 2);
                if (entry.Type == EntriesType.Directory)
                {
                    displayString += $"[ {entry.Name} ]";
                    displayList.Add(displayString);
                }
                else
                {
                    displayString += $"{entry.Name}";
                    displayList.Add(displayString);
                }
                if (entry.Type == EntriesType.Directory && recursionCount > 1)
                {
                    MakeDisplayList(GetEntriesList(entry.Path), recursionCount - 1);
                }
            }
        }
        /// <summary>
        /// Создаёт список строк необходимых вывести на косоль постранично с учетом указанного количества отображаемых на странице элементов.
        /// </summary>
        static void MakePages()
        {
            int currentItem = 1;
            int pagesCount = displayList.Count / rowsToDisplay;
            if (displayList.Count % rowsToDisplay > 0)
            {
                pagesCount++;
            }
            int currentPage = 0;
            displayPages = new List<string>[pagesCount];
            for (int i = 0; i < pagesCount; i++)
            {
                displayPages[i] = new List<string>();
            }
            foreach (string displayString in displayList)
            {
                displayPages[currentPage].Add(displayString);
                currentItem++;
                if (currentItem > rowsToDisplay * (currentPage + 1))
                {
                    currentPage++;
                }
            }
            currentDisplayPage = 0;
        }
        /// <summary>
        /// Текущая выбранная страница увеличивается на 1.
        /// </summary>
        static void NextPage()
        {
            if (currentDisplayPage < displayPages.Length - 1)
            {
                currentDisplayPage++;
            }
        }
        /// <summary>
        /// Текущая выбранная страница уменьшается на 1.
        /// </summary>
        static void PrevPage()
        {
            if (currentDisplayPage > 0)
            {
                currentDisplayPage--;
            }
        }
        /// <summary>
        /// Меняет текущую выбранную директорию и сохраняет в конфиг.
        /// </summary>
        /// <param name="newDirectory">Путь к новой директории.</param>
        /// <param name="userInteraction">Нужно ли выводить в инфопанель сообщения.</param>
        static void ChangeCurrentDirectory(string newDirectory, bool userInteraction)
        {
            newDirectory = MakeAbsolutePath(newDirectory);
            newDirectory = GetBackDirectory(newDirectory);
            if (Directory.Exists(newDirectory))
            {
                currentDirectory = newDirectory;
                displayList.Clear();
                MakeDisplayList(GetEntriesList(newDirectory), recursionLevel);
                MakePages();
                if (userInteraction)
                {
                    infoArea.DisplayLine($"Текущая директория применена: {newDirectory}");
                }
                Config.Config.WriteLastDir(newDirectory);
            }
            else
            {
                if (userInteraction)
                {
                    infoArea.DisplayLine("Нет такой директории");
                }
            }
        }
        /// <summary>
        /// Проверяет является ли строка абсолютным путём.
        /// </summary>
        /// <param name="str">Строка, которую необходимо проверить.</param>
        /// <returns>true если строка это абсолютный путь, false в остальных случаях</returns>
        static bool IsStringDir(string str)
        {
            Regex regex = new Regex(@"^(\w):\\.*");
            if (regex.IsMatch(str))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Убирает разделитель директории в начале абсолютного пути, если он есть
        /// </summary>
        /// <param name="path">Строка.</param>
        /// <returns>Отредактированная строка.</returns>
        static string TrimStartingDirectorySeparator(string path)
        {
            Regex regex = new Regex(@"^\\.*");
            if (regex.IsMatch(path))
            {
                return path.Remove(0, 1);
            }
            return path;
        }
        /// <summary>
        /// Возвращает путь уровнем выше, если в конце был указан ".."
        /// </summary>
        /// <param name="path">Строка.</param>
        /// <returns>Отредактированная строка.</returns>
        static string GetBackDirectory(string path)
        {
            Regex regex = new Regex(@"\\\.{2}$");
            if (regex.IsMatch(path))
            {
                path = regex.Replace(path, "");
                path = Path.GetDirectoryName(path);
            }
            return path;
        }
        /// <summary>
        /// Недоделанный метод)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static string TrimBackDirectory(string path)
        {
            Regex regex = new Regex(@"\\\.{2}$");
            if (regex.IsMatch(path))
            {
                path = regex.Replace(path, "");
                path = Path.GetDirectoryName(path);
            }
            return path;
        }
        /// <summary>
        /// Редактирует путь относительный или абсолютный, приводя к одинаковому виду.
        /// </summary>
        /// <param name="path">Путь.</param>
        /// <returns>Отредактированный путь.</returns>
        static string MakeAbsolutePath(string path)
        {
            if (!IsStringDir(path))
            {
                path = TrimStartingDirectorySeparator(path);
                path = Path.Combine(currentDirectory, path);
            }
            path = path.ToLower();
            path =  Path.TrimEndingDirectorySeparator(path);
            path = GetBackDirectory(path);
            return path;
        }
        /// <summary>
        /// Возвращает имя объекта с приставкой "_copy"
        /// </summary>
        /// <param name="path">Строка.</param>
        /// <returns>Отредактированная строка.</returns>
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
        /// <summary>
        /// Method for copy source to target.
        /// </summary>
        /// <param name="source">Can be a name of file in CD, path relative to CD or full path.</param>
        /// <param name="target">Can be path relative to CD or full path</param>
        /// <param name="userInteraction">Нужно ли выводить в инфопанель сообщения.</param>
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
        /// <summary>
        /// Вспомогательный метод для копирования папок.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
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
        /// <summary>
        /// Метод для перемещения объекта.
        /// </summary>
        /// <param name="source">Can be a name of file in CD, path relative to CD or full path.</param>
        /// <param name="target">Can be path relative to CD or full path</param>
        /// <param name="userInteraction">Нужно ли выводить в инфопанель сообщения.</param>
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
                                    ChangeCurrentDirectory(currentDirectory, false);
                                }
                                else
                                {
                                    infoArea.DisplayLine("Файл не перемещен.");
                                }
                            }
                            else
                            {
                                File.Move(source, target, true);
                                ChangeCurrentDirectory(currentDirectory, false);
                            }
                        }
                        else
                        {
                            File.Move(source, target);
                            if (userInteraction)
                            {
                                infoArea.DisplayLine("Файл перемещен.");
                            }
                            ChangeCurrentDirectory(currentDirectory, false);
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
                                    infoArea.DisplayLine("Папка перемещена с заменой");
                                    ChangeCurrentDirectory(currentDirectory, false);
                                }
                                else
                                {
                                    infoArea.DisplayLine("Папка не перемещена!");
                                }
                            }
                            else
                            {
                                MoveFolder(source, target);
                                ChangeCurrentDirectory(currentDirectory, false);
                            }
                        }
                        else
                        {
                            MoveFolder(source, target);
                            if (userInteraction)
                            {
                                infoArea.DisplayLine("Папка перемещена");
                            }
                            ChangeCurrentDirectory(currentDirectory, false);
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
        /// <summary>
        /// Вспомогательный метод для перемещения папок.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
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
        /// <summary>
        /// Метод для удаления объекта
        /// </summary>
        /// <param name="path">Can be a name of file in CD, path relative to CD or full path.</param>
        /// <param name="userInteraction">Нужно ли выводить в инфопанель сообщения.</param>
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
                        if (userInteraction)
                        {
                            infoArea.DisplayLine("Файл удален");
                        }
                    }
                    ChangeCurrentDirectory(currentDirectory, false);
                }
                else
                {
                    File.Delete(path);
                    if (userInteraction)
                    {
                        infoArea.DisplayLine("Файл удален");
                    }
                    ChangeCurrentDirectory(currentDirectory, false);
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
                        ChangeCurrentDirectory(currentDirectory, false);
                    }
                }
                else
                {
                    DeleteFolder(path);
                    if (userInteraction)
                    {
                        infoArea.DisplayLine("Папка удалена");
                    }
                    ChangeCurrentDirectory(currentDirectory, false);
                }
            }
            else
            {
                infoArea.DisplayLine("Неверно задано имя объекта для удаления!");
            }
        }
        /// <summary>
        /// Вспомогательный метод для удаления папок.
        /// </summary>
        /// <param name="path"></param>
        static void DeleteFolder(string path)
        {
            string[] entries = Directory.GetFileSystemEntries(path);
            foreach (string item in entries)
            {
                Delete(item, false);
            }
            Directory.Delete(path);
        }
        /// <summary>
        /// Метод для отображения атрибутов объекта.
        /// </summary>
        /// <param name="path">Путь к объекту.</param>
        static void ShowInfo(string path)
        {
            path = MakeAbsolutePath(path);
            // Если это папка
            if (Directory.Exists(path))
            {
                DateTime creationTime = Directory.GetCreationTime(path);
                int dirCount = Directory.GetDirectories(path).Length;
                int fileCount = Directory.GetFiles(path).Length;
                infoArea.DisplayLine($"Дата создания: {creationTime}",
                    $"Папок: {dirCount}",
                    $"Файлов: {fileCount}");
            }
            // Если это файл
            else if (File.Exists(path))
            {
                DateTime creationTime = File.GetCreationTime(path);
                DateTime changingTime = File.GetLastWriteTime(path);
                FileInfo fileinfo = new FileInfo(path);
                infoArea.DisplayLine($"Дата создания: {creationTime}",
                    $"Дата изменения: {changingTime}",
                    $"Размер: {fileinfo.Length} Байт");
            }
            else
            {
                infoArea.DisplayLine("Указанный объект не существует!");
            }
        }
        /// <summary>
        /// Отобразить список команд.
        /// </summary>
        static void ShowHelp()
        {
            infoArea.Clear();
            infoArea.DisplayLine("cd [target] - задать задать директорию;", 
                "copy | cp [source] [target] - копировать объект;", 
                "move | mv [source] [target] - переместить объект;", 
                "remove | rm [target] - удалить объект;",
                "info [target] - показать атрибуты объекта;",
                "< | > - для выбора страниц отображения;",
                "exit - завершение работы.");
        }
        /// <summary>
        /// Записать исключение в лог.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        static void LogException(Exception exception)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "error.txt");
            //if (!File.Exists(path))
            //{
            //    File.Create(path);
            //}
            File.AppendAllText(path, $"{DateTime.Now.ToString()}: {exception.Message}\n");
        }
        static void Main()
        {
            // Чтение настроек
            bool isNeedToExit = false;
            Config.Config.ReadConfig();
            rowsToDisplay = Config.Config.RowsToDisplay;
            recursionLevel = Config.Config.RecursionLevel;
            
            // UI
            treeArea = new Square(0, 0, Console.BufferWidth - 1, rowsToDisplay + 2);
            treeArea.bottomLeftCornerSymbol = '╠';
            treeArea.bottomRightCornerSymbol = '╣';
            treeArea.MakeAllBorders();
            treeArea.Draw();
            infoArea = new Square(0, treeArea.posBorderBottom, Console.BufferWidth - 1, 9);
            infoArea.topLeftCornerSymbol = '╠';
            infoArea.topRightCornerSymbol = '╣';
            infoArea.bottomLeftCornerSymbol = '╠';
            infoArea.bottomRightCornerSymbol = '╣';
            infoArea.MakeAllBorders();
            infoArea.Draw();
            consoleArea = new Square(0, infoArea.posBorderBottom, Console.BufferWidth - 1, 3);
            consoleArea.topLeftCornerSymbol = '╠';
            consoleArea.topRightCornerSymbol = '╣';
            consoleArea.MakeAllBorders();
            consoleArea.Draw();
            Console.WriteLine();

            // Применение последней директории
            try
            {
                ChangeCurrentDirectory(Config.Config.LastDir, false);
            }
            catch (Exception e)
            {
                LogException(e);
                ChangeCurrentDirectory(defaultDir, false);
            }
            infoArea.DisplayLine("Введите \"help\" для отображения списка команд.");
            
            // Бесконечный цикл
            do
            {
                try
                {
                    treeArea.DisplayList(displayPages[currentDisplayPage]);
                }
                catch (Exception e)
                {
                    LogException(e);
                    ChangeCurrentDirectory(defaultDir, false);
                    treeArea.DisplayList(displayPages[currentDisplayPage]);
                }
                try
                {
                    isNeedToExit = ParseCommand(consoleArea.Input($"{currentDirectory}>"));
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            } while (isNeedToExit != true);
        }
    }
}
