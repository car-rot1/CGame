using System;
using System.IO;
using System.Linq;

namespace CGame
{
    [Flags]
    public enum PathState
    {
        UnKnown = 0,
        Invalid = 1 << 0,
        Directory = 1 << 1,
        File = 1 << 2,
        Exist = 1 << 3,
        UnExist = 1 << 4,
    }
    
    public static class PathExtension
    {
        public static PathState GetPathState(this string self)
        {
            if (string.IsNullOrWhiteSpace(self))
                return PathState.Invalid;

            var fileName = Path.GetFileName(self);
            if (string.IsNullOrWhiteSpace(fileName) || !Path.HasExtension(fileName))
            {
                if (Path.GetInvalidPathChars().Any(self.Contains))
                    return PathState.Invalid;

                return PathState.Directory | (Directory.Exists(self) ? PathState.Exist : PathState.UnExist);
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(self);
            if (string.IsNullOrWhiteSpace(fileNameWithoutExtension) || Path.GetInvalidFileNameChars().Any(fileName.Contains))
                return PathState.Invalid;
            
            return PathState.File | (File.Exists(self) ? PathState.Exist : PathState.UnExist);
        }
    }
}