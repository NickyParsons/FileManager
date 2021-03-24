using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace FileManager.Config
{
    class Config
    {
        public static string ConfigFilePath { get; set; }
        public static int RowsToDisplay { get; set; }
        public static int RecursionLevel { get; set; }
        public static string LastDir { get; set; }

        static Config()
        {
            ConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "config.xml");
        }
        public static void ReadConfig()
        {
            if (!File.Exists(ConfigFilePath))
            {
                CreateConfigFIle();
            }
            XmlDocument xml = new XmlDocument();
            xml.Load(ConfigFilePath);
            XmlElement root = xml.DocumentElement;
            RowsToDisplay = Convert.ToInt32(root.GetAttribute("rowsToDisplay"));
            RecursionLevel = Convert.ToInt32(root.GetAttribute("recursionLevel"));
            LastDir = root.GetAttribute("lastDir");
        }

        public static void CreateConfigFIle()
        {
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("Setting");
            root.SetAttribute("rowsToDisplay", "18");
            root.SetAttribute("recursionLevel", "2");
            root.SetAttribute("lastDir", "D:\\");
            xml.AppendChild(root);
            xml.Save(ConfigFilePath);
        }

        public static void WriteLastDir(string path)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(ConfigFilePath);
            XmlElement root = xml.DocumentElement;
            root.SetAttribute("lastDir", path);
            xml.Save(ConfigFilePath);
        }

    }
}
