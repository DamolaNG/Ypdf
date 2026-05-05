using RuntimeLib.Python;
using YpdfLib.Models.Imaging;

namespace YpdfLib.Compressors
{
    public static class ImageCompressor
    {
        private const bool CONVERT_COMMA_TO_DOT = true;

        public static void Compress(string inputFile, string destPath, string? pythonAlias = null,
            TextWriter? outputWriter = null)
        {
            Compress(inputFile, destPath, new ImageCompression(), pythonAlias, outputWriter);
        }

        public static void Compress(string inputFile, string destPath, IImageCompression compression,
            string? pythonAlias = null, TextWriter? outputWriter = null)
        {
            var executor = new PythonExecutor(true, true, outputWriter)
            {
                RequirePython3 = true
            };

            if (!string.IsNullOrEmpty(pythonAlias))
                executor.PythonAlias = pythonAlias;

            string sizeFactor = ConvertFloatToString(compression.SizeFactor);
            string qualityFactor = ConvertFloatToString(compression.QualityFactor);

            string pythonImageCompressorPath = SharedConfig.Scripts.PythonImageCompressor;

            var args = new List<string>
            {
                pythonImageCompressorPath,
                "-i", inputFile,
                "-o", destPath,
                "-q", qualityFactor,
                "-s", sizeFactor
            };

            if (compression.Width is not null)
            {
                args.Add("-W");
                args.Add(compression.Width.ToString()!);
            }

            if (compression.Height is not null)
            {
                args.Add("-H");
                args.Add(compression.Height.ToString()!);
            }

            executor.Execute(args);
        }

        public static void Compress(string[] inputFiles, string destDir, string? pythonAlias = null,
            TextWriter? outputWriter = null)
        {
            Compress(inputFiles, destDir, new ImageCompression(), pythonAlias, outputWriter);
        }

        public static void Compress(string[] inputFiles, string destDir, IImageCompression compression,
            string? pythonAlias = null, TextWriter? outputWriter = null)
        {
            if (inputFiles.Length == 0)
                return;

            var executor = new PythonExecutor(true, true, outputWriter)
            {
                RequirePython3 = true
            };

            if (!string.IsNullOrEmpty(pythonAlias))
                executor.PythonAlias = pythonAlias;

            string pythonImageCompressorPath = SharedConfig.Scripts.PythonImageCompressor;

            string sizeFactor = ConvertFloatToString(compression.SizeFactor);
            string qualityFactor = ConvertFloatToString(compression.QualityFactor);

            var args = new List<string> { pythonImageCompressorPath, "-i" };
            args.AddRange(inputFiles);

            args.Add("-O");
            args.Add(destDir ?? string.Empty);

            args.Add("-q");
            args.Add(qualityFactor);
            args.Add("-s");
            args.Add(sizeFactor);

            if (!string.IsNullOrEmpty(compression.Extension))
            {
                args.Add("-e");
                args.Add(compression.Extension);
            }

            executor.Execute(args);
        }

        private static string ConvertFloatToString(float value)
        {
            return CONVERT_COMMA_TO_DOT
                ? value.ToString().Replace(',', '.')
                : value.ToString();
        }
    }
}
