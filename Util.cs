using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using System;

namespace EventRemembrance
{
    class Util
    {
        // http://stackoverflow.com/questions/14238461/xna-content-directory-iteration
        public static Dictionary<string, T> LoadContentFolder<T>(string contentFolder)
        {
            ContentManager contentManager = StardewValley.Game1.content;

            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            Dictionary<string, T> result = new Dictionary<string, T>();

            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                result.Add(file.Name, contentManager.Load<T>(contentFolder + "/" + file.Name.Split('.')[0]));
            }
            return result;
        }
    }
}
