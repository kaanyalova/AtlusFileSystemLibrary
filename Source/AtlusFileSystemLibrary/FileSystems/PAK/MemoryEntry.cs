﻿using System.IO;

namespace AtlusFileSystemLibrary.FileSystems.PAK
{
    internal class MemoryEntry : IEntry
    {
        private readonly Stream mStream;
        private readonly bool mOwnsStream;

        public string FileName { get; }

        public int Length => ( int )mStream.Length;

        public MemoryEntry( Stream stream, bool ownsStream, string fileName )
        {
            mStream = stream;
            mOwnsStream = ownsStream;
            FileName = fileName;
        }

        public Stream GetStream()
        {
            return mStream;
        }

        public void Dispose()
        {
            if (mOwnsStream)
                mStream?.Dispose();
        }
    }
}