namespace System.IO
{
    public static class DirectoryInfoExtensions
    {
        public static DirectoryInfo GetChildDirectoryInfo(this DirectoryInfo directoryInfo, string childDirenctPath)
        {
            if (OperatingSystem.IsWindows())
                return new DirectoryInfo($"{directoryInfo.FullName}\\{childDirenctPath}");
            else
                return new DirectoryInfo($"{directoryInfo.FullName}/{childDirenctPath}");
        }

        public static FileInfo GetFileInfo(this DirectoryInfo directoryInfo, string filePath)
        {
            if (OperatingSystem.IsWindows())
                return new FileInfo($"{directoryInfo.FullName}\\{filePath}");
            else
                return new FileInfo($"{directoryInfo.FullName}/{filePath}");
        }
    }
}
