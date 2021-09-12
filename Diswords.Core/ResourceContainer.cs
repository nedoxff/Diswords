using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.Fonts;

namespace Diswords.Core
{
    public static class ResourceContainer
    {
        public static Dictionary<string, FontFamily> FontFamilies = new();
        private static FontCollection _fontCollection = new();
        
        
        public static void Load()
        {
            if (!Directory.Exists("Resources"))
                throw new Exception("The resources directory does not exist!");

            foreach (var file in Directory.GetFiles("Resources", "*.ttf", SearchOption.AllDirectories))
                FontFamilies[Path.GetFileNameWithoutExtension(file)] = _fontCollection.Install(file);
            
        }
    }
}