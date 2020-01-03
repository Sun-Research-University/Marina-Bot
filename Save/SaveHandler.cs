﻿using RK800.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace RK800.Save
{
    class SaveHandler
    {
        public static readonly DirectoryInfo save = new DirectoryInfo("save");

        public static Dictionary<string, ISaveFile> Saves = new Dictionary<string, ISaveFile>();

        private static readonly string[] PreDefinedSaves = { "Trackers.Tracker", "Filters.UlongStringList", "Warns.Warn", "LogChannels.UlongUlong" };

        public static FilterSaveFile FilterSave => SaveHandler.Saves["Filters"] as FilterSaveFile;
        public static WarnSaveFile WarnsSave => SaveHandler.Saves["Warns"] as WarnSaveFile;
        public static UlongUlongSaveFile LogChannelsSave => SaveHandler.Saves["LogChannels"] as UlongUlongSaveFile;
        public static TrackerSaveFile TrackersSave => SaveHandler.Saves["Trackers"] as TrackerSaveFile;

        private static ISaveFile OpenSaveFile(FileInfo file)
        {
            return (file.Extension.ToLower()) switch
            {
                ".tracker" => new TrackerSaveFile(file),
                ".ulongstringlist" => new FilterSaveFile(file),
                ".warn" => new WarnSaveFile(file),
                ".ulongulong" => new UlongUlongSaveFile(file),
                _ => throw new Exception("File not a save!"),
            };
        }

        public static void Populate()
        {
            save.Create();
            foreach (string str in PreDefinedSaves)
            {
                FileInfo info = save.GetFile(str);
                if (!info.Exists)
                    info.Create().Close();
            }

            foreach (FileInfo file in save.EnumerateFiles())
                Saves[Path.GetFileNameWithoutExtension(file.FullName)] = OpenSaveFile(file);

            foreach (ISaveFile file in Saves.Values)
                file.Read();
        }

        public static void SaveAll()
        {
            foreach (ISaveFile save in Saves.Values)
                save.Write();
        }
    }
}
