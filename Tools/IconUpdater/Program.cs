using System.CommandLine;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var foregroundOption = new Option<FileInfo?>(
            name: "--foreground",
            description: "The icon foreground image.");

        var backgroundOption = new Option<FileInfo?>(
            name: "--background",
            description: "The icon background image.");

        var backgroundColorOption = new Option<string?>(
            name: "--background-color",
            description: "The icon background color.");

        var outputOption = new Option<DirectoryInfo?>(
            name: "--output",
            description: "Output directory.");

        var foregroundRoundScaleOption = new Option<float?>(
            name: "--foreground-round-scale",
            description: "The icon foreground image scale when rendered for round icon.");

        var rootCommand = new RootCommand("Generate icons from a single image");
        rootCommand.AddOption(foregroundOption);
        rootCommand.AddOption(backgroundOption);
        rootCommand.AddOption(backgroundColorOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(foregroundRoundScaleOption);

        rootCommand.SetHandler(GenerateIcons,
            foregroundOption, backgroundOption, backgroundColorOption, outputOption, foregroundRoundScaleOption);

        return await rootCommand.InvokeAsync(args);
    }

    private static void GenerateIcons(FileInfo? fore, FileInfo? back, string? backCol, DirectoryInfo? output, float? foregroundRoundScale)
    {
        Color backgroundColor = Color.Transparent;
        if (!string.IsNullOrWhiteSpace(backCol))
            backgroundColor = System.Drawing.ColorTranslator.FromHtml(backCol);

        Image? foreground = (fore != null && fore.Exists) ? Image.FromFile(fore.FullName) : null;
        Image? background = (back != null && back.Exists) ? Image.FromFile(back.FullName) : null;

        var outPath = (output == null)?Directory.GetCurrentDirectory(): output.FullName;

        MakeWindowsIcon(outPath, background, backgroundColor, foreground);
        MakeUWPIcons(outPath, background, backgroundColor, foreground);
        MakeAndroidIcons(outPath, background, backgroundColor, foreground, foregroundRoundScale ?? 0.7071f);
        MakeIOSIcons(outPath, background, backgroundColor, foreground);
    }

    private static void MakeWindowsIcon(string outPath, Image? background, Color backgroundColor, Image? foreground)
    {
        MakePng(Path.Combine(outPath, "RbfxTemplate.Desktop\\icon.ico"), 64, 64, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
    }

    private static void MakeUWPIcons(string outPath, Image? background, Color backgroundColor, Image? foreground)
    {
        MakePng(Path.Combine(outPath, "RbfxTemplate.UWP\\Assets\\LockScreenLogo.scale-200.png"), 48, 48, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.UWP\\Assets\\SplashScreen.scale-200.png"), 1240, 600, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.UWP\\Assets\\Square150x150Logo.scale-200.png"), 300, 300, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.UWP\\Assets\\Square44x44Logo.scale-200.png"), 88, 88, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.UWP\\Assets\\Square44x44Logo.targetsize-24_altform-unplated.png"), 24, 24, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.UWP\\Assets\\StoreLogo.png"), 50, 50, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.UWP\\Assets\\Wide310x150Logo.scale-200.png"), 620, 300, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
    }
    private static void MakeAndroidIcons(string outPath, Image? background, Color backgroundColor, Image? foreground, float foregroundRoundScale)
    {
        // hdpi

        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-hdpi\\ic_launcher.png"), 72, 72, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-hdpi\\ic_launcher_foreground.png"), 162, 162, (Bitmap bmp) =>
        {
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-hdpi\\ic_launcher_round.png"), 72, 72, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground, foregroundRoundScale);
        });

        // mdpi

        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-mdpi\\ic_launcher.png"), 48, 48, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-mdpi\\ic_launcher_foreground.png"), 108, 108, (Bitmap bmp) =>
        {
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-mdpi\\ic_launcher_round.png"), 48, 48, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground, foregroundRoundScale);
        });

        // xhdpi

        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-xhdpi\\ic_launcher.png"), 96, 96, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-xhdpi\\ic_launcher_foreground.png"), 216, 216, (Bitmap bmp) =>
        {
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-xhdpi\\ic_launcher_round.png"), 96, 96, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground, foregroundRoundScale);
        });

        // xxhdpi

        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-xxhdpi\\ic_launcher.png"), 144, 144, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-xxhdpi\\ic_launcher_foreground.png"), 324, 324, (Bitmap bmp) =>
        {
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-xxhdpi\\ic_launcher_round.png"), 144, 144, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground, foregroundRoundScale);
        });

        // xxxhdpi

        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-xxxhdpi\\ic_launcher.png"), 192, 192, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-xxxhdpi\\ic_launcher_foreground.png"), 432, 432, (Bitmap bmp) =>
        {
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.Android\\Resources\\mipmap-xxxhdpi\\ic_launcher_round.png"), 192, 192, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground, foregroundRoundScale);
        });
    }
    private static void MakeIOSIcons(string outPath, Image? background, Color backgroundColor, Image? foreground)
    {

        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon1024.png"), 1024, 1024, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon120.png"), 120, 120, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon152.png"), 152, 152, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon167.png"), 167, 167, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon180.png"), 180, 180, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon20.png"), 20, 20, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon29.png"), 29, 29, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon40.png"), 40, 40, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon58.png"), 58, 58, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon60.png"), 60, 60, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon76.png"), 76, 76, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon80.png"), 80, 80, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
        MakePng(Path.Combine(outPath, "RbfxTemplate.IOS\\Assets.xcassets\\AppIcon.appiconset\\Icon87.png"), 87, 87, (Bitmap bmp) =>
        {
            RenderBackground(bmp, background, backgroundColor);
            RenderForeground(bmp, foreground);
        });
    }

    private static void RenderBackground(Bitmap bmp, Image? background, Color backgroundColor)
    {
        using (var gr = Graphics.FromImage(bmp))
        {
            if (background == null)
            {
                gr.Clear(backgroundColor);
            }
            else
            {
                RectangleF srcRect = new RectangleF(0, 0, background.Width, background.Height);
                RectangleF dstRect = new RectangleF(0, 0, bmp.Width, bmp.Height);

                srcRect = CropRectangle(srcRect, dstRect);

                gr.DrawImage(background, dstRect, srcRect, GraphicsUnit.Pixel);
            }
        }
    }

    private static RectangleF CropRectangle(RectangleF srcRect, RectangleF dstRect)
    {
        var dstAspect = dstRect.Width / dstRect.Height;
        var srcAspect = srcRect.Width / srcRect.Height;

        if (srcAspect == dstAspect)
            return srcRect;

        var srcMidX = srcRect.X + srcRect.Width / 2.0f;
        var srcMidY = srcRect.Y + srcRect.Height / 2.0f;

        if (srcAspect > dstAspect)
        {
            var newWidth = srcRect.Height * dstAspect;
            return new RectangleF(srcMidX - newWidth * 0.5f, srcRect.Y, newWidth, srcRect.Height);
        }
        else
        {
            var newHeight = srcRect.Width / dstAspect;
            return new RectangleF(srcRect.X, srcMidY - newHeight*0.5f, srcRect.Width, newHeight);
        }
    }
    private static RectangleF FitRectangle(RectangleF srcRect, RectangleF dstRect)
    {
        var dstAspect = dstRect.Width / dstRect.Height;
        var srcAspect = srcRect.Width / srcRect.Height;

        if (srcAspect == dstAspect)
            return dstRect;

        var dstMidX = dstRect.X + dstRect.Width / 2.0f;
        var dstMidY = dstRect.Y + dstRect.Height / 2.0f;

        if (srcAspect > dstAspect)
        {
            var newHeight = dstRect.Width / srcAspect;
            return new RectangleF(dstRect.X, dstMidY - newHeight * 0.5f, dstRect.Width, newHeight);
        }
        else
        {
            var newWidth = dstRect.Height * srcAspect;
            return new RectangleF(dstMidX - newWidth * 0.5f, dstRect.Y, newWidth, dstRect.Height);
        }
    }

    private static void RenderForeground(Bitmap bmp, Image? foreground, float scale = 1.0f)
    {
        if (foreground != null)
        {
            using (var gr = Graphics.FromImage(bmp))
            {
                RectangleF srcRect = new RectangleF(0, 0, foreground.Width, foreground.Height);
                RectangleF dstRect = new RectangleF(0, 0, bmp.Width, bmp.Height);

                dstRect = FitRectangle(srcRect, dstRect);

                dstRect = ScaleRectangle(dstRect, scale);

                gr.DrawImage(foreground, dstRect, srcRect, GraphicsUnit.Pixel);
            }
        }
    }

    private static RectangleF ScaleRectangle(RectangleF dstRect, float scale)
    {
        var newWidth = dstRect.Width * scale;
        var newHeight = dstRect.Height * scale;

        return new RectangleF(dstRect.X+(dstRect.Width- newWidth)*0.5f, dstRect.Y + (dstRect.Height - newHeight) * 0.5f, newWidth, newHeight);
    }

    private static void MakePng(string path, int width, int height, Action<Bitmap> recipe)
    {
        using (var bmp = new Bitmap(width, height))
        {
            recipe(bmp);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            switch (Path.GetExtension(path).ToLower())
            {
                case ".png":
                    bmp.Save(path);
                    break;
                case ".ico":
                    SaveIco(path, bmp);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    private static void SaveIco(string path, Bitmap bmp)
    {
        using (var fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            using (var binaryWriter = new BinaryWriter(fileStream))
            {
                var dir = new IconDir()
                {
                    Reserved = 0,
                    ImageType = 1,
                    NumberOfImages = 1
                };

                binaryWriter.Write(dir.Reserved);
                binaryWriter.Write(dir.ImageType);
                binaryWriter.Write(dir.NumberOfImages);

                var ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Png);
                ms.Flush();
                var bmpData = ms.ToArray();

                var iconDirEntry = new IconDirEntry()
                {
                    Width = (bmp.Width == 256) ? (byte)0 : (byte)bmp.Width,
                    Height = (bmp.Height == 256) ? (byte)0 : (byte)bmp.Height,
                    Palette = 0,
                    Reserved = 0,
                    ColorPlanes = 0,
                    BitsPerPixel = 32,
                    ImageDataSize = (uint)bmpData.Length,
                    ImageOffset = 22,
                };

                binaryWriter.Write(iconDirEntry.Width);
                binaryWriter.Write(iconDirEntry.Height);
                binaryWriter.Write(iconDirEntry.Palette);
                binaryWriter.Write(iconDirEntry.Reserved);
                binaryWriter.Write(iconDirEntry.ColorPlanes);
                binaryWriter.Write(iconDirEntry.BitsPerPixel);
                binaryWriter.Write(iconDirEntry.ImageDataSize);
                binaryWriter.Write(iconDirEntry.ImageOffset);

                binaryWriter.Write(bmpData);
            }
        }
    }

    private static void GenerateCode(DirectoryInfo output)
    {
        var outputFullName = output.FullName;
        var png = Directory.EnumerateFiles(outputFullName, "*.png", SearchOption.AllDirectories);
        foreach (var path in png)
        {
            var bmp = Image.FromFile(path);
            Console.WriteLine($"MakePng(\"{path.Substring(outputFullName.Length).Replace("\\","\\\\")}\", {bmp.Width}, {bmp.Height}, (Bitmap bmp) =>");
            Console.WriteLine("{");
            if (!png.Contains("_foreground"))
                Console.WriteLine("    RenderBackground(bmp, background, backgroundColor);");
            Console.WriteLine("    RenderForeground(bmp, foreground);");
            Console.WriteLine("});");
        }
    }


}

[StructLayout(LayoutKind.Sequential)]
internal struct IconDirEntry
{
    public byte Width;
    public byte Height;
    public byte Palette;
    public byte Reserved;
    public ushort ColorPlanes;
    public ushort BitsPerPixel;
    public uint ImageDataSize;
    public uint ImageOffset;
}