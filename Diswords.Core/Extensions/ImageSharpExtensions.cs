using SixLabors.Fonts;

namespace Diswords.Core.Extensions
{
    public static class ImageSharpExtensions
    {
        public static Font CreateFont(this FontFamily family, float size)
        {
            return new(family, size);
        }
    }
}