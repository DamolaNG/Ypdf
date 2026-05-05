using RuntimeLib.Python;

namespace YpdfLib.Extractors
{
    public static class ImageExtractor
    {
        public static void Extract(string destDir, string[] inputFiles, int extractedImagesLimit = 0, TextWriter? outputWriter = null)
        {
            Extract(destDir, null, inputFiles, extractedImagesLimit, outputWriter);
        }

        public static void Extract(string destDir, string? pythonAlias, string[] inputFiles, int extractedImagesLimit = -1, TextWriter? outputWriter = null)
        {
            if (inputFiles is null)
                throw new ArgumentNullException(nameof(inputFiles));

            if (inputFiles.Length == 0)
                return;

            string pythonImageExtractorPath = SharedConfig.Scripts.PythonImageExtractor;

            var executor = new PythonExecutor(true, true, outputWriter)
            {
                RequirePython3 = true,
                ErrorDataVerifier = t => !string.IsNullOrEmpty(t),
                ErrorDataConverter = t => t != null && t.Contains("pages") ? $"\n{t}" : t
            };

            if (!string.IsNullOrEmpty(pythonAlias))
                executor.PythonAlias = pythonAlias;

            var args = new List<string>
            {
                pythonImageExtractorPath,
                "-l", extractedImagesLimit.ToString(),
                "-o", destDir ?? string.Empty,
                "-i"
            };
            args.AddRange(inputFiles);

            executor.Execute(args);
        }
    }
}
