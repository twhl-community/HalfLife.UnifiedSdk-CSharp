using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using Sledge.Formats.Id;

namespace HalfLife.UnifiedSdk.Bsp2Obj
{
    internal sealed class TextureWriter
    {
        private readonly ILogger _logger;

        private readonly string _destinationDirectory;

        private readonly TgaEncoder _encoder = new()
        {
            Compression = TgaCompression.None,
            BitsPerPixel = TgaBitsPerPixel.Pixel24
        };

        public TextureWriter(ILogger logger, string destinationDirectory)
        {
            _logger = logger;
            _destinationDirectory = destinationDirectory;
        }

        public void Write(string fileName, MipTexture texture)
        {
            var pixels = texture.MipData[0]
                .Select(i => new Rgb24(
                    texture.Palette[i * 3],
                    texture.Palette[(i * 3) + 1],
                    texture.Palette[(i * 3) + 2]))
                .ToArray();

            Image<Rgb24> image = Image.LoadPixelData<Rgb24>(pixels, (int)texture.Width, (int)texture.Height);

            var absoluteFileName = Path.Combine(_destinationDirectory, fileName);

            _logger.Information("Writing embedded texture {TextureName} to {FileName}", texture.Name, fileName);

            using var stream = File.Open(absoluteFileName, FileMode.Create);

            _encoder.Encode(image, stream);
        }
    }
}
