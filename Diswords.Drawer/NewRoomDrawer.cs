using System.IO;
using Diswords.Core;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Diswords.Drawer
{
    public class NewRoomDrawer
    {
        public static MemoryStream Generate(string label, string id, bool isPrivate)
        {
            var result = new MemoryStream();

            var gothamFamily = ResourceContainer.FontFamilies["gotham"];
            var gothamBig = new Font(gothamFamily, 96f);
            var gothamMedium = new Font(gothamFamily, 48f);

            var image = new Image<Rgba32>(800, 600);

            image.Mutate(i =>
            {
                i.Clear(new GraphicsOptions(), Color.FromRgba(184, 232, 164, 255));
                i.DrawText(new DrawingOptions
                {
                    TextOptions = new TextOptions
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                }, id, gothamBig, Brushes.Solid(Color.White), new Pen(Color.Black, 3f), new PointF(400, 300));
                /*i.DrawText(new DrawingOptions
                {
                    TextOptions = new TextOptions
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top
                    }
                })*/
            });

            image.SaveAsPng(result);
            image.Dispose();

            result.Seek(0, SeekOrigin.Begin);
            return result;
        }
    }
}