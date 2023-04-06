


namespace PAKPackLib;


using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using AtlusFileSystemLibrary;
using AtlusFileSystemLibrary.Common.IO;
using AtlusFileSystemLibrary.FileSystems.PAK;
using static AtlusFileSystemLibrary.FileSystems.PAK.FormatVersion;

public static class lib
{

    [UnmanagedCallersOnly(EntryPoint = "unpack_pak")]
    public static int unpack_pak(IntPtr in_path_ptr, IntPtr out_path_ptr)
    {
        string inputPath;
        string outputPath;

        try
        {
            inputPath = Marshal.PtrToStringAnsi(in_path_ptr);
            outputPath = Marshal.PtrToStringAnsi(out_path_ptr);

        }
        catch
        {
            return -1;
        }





        if (!File.Exists(inputPath))
        {
            return -1; // in file path does not exist
        }


        Directory.CreateDirectory(outputPath);

        if (!PAKFileSystem.TryOpen(inputPath, out var pak))
        {
            return -1;
        }

        using (pak)
        {
            //Console.WriteLine($"PAK format version: {Program.FormatVersionEnumToString[pak.Version]}");

            foreach (string file in pak.EnumerateFiles())
            {
                var normalizedFilePath = file.Replace("../", ""); // Remove backwards relative path
                using (var stream = FileUtils.Create(outputPath + Path.DirectorySeparatorChar + normalizedFilePath))
                using (var inputStream = pak.OpenFile(file))
                {
                    inputStream.CopyTo(stream);
                }
            }
        }

        return 1;
    }

    [UnmanagedCallersOnly(EntryPoint = "add")]
    public static int Add(int a, int b)
    {
        return a + b;
    }

    // todo add packing function



    [UnmanagedCallersOnly(EntryPoint = "pack_pak")]
    public static int pack_pak(IntPtr in_path_ptr, IntPtr out_path_ptr, IntPtr format_ptr)
    {
        string inputPath;
        string outputPath;
        string _format_str;

        try
        {
            inputPath = Marshal.PtrToStringAnsi(in_path_ptr);
            outputPath = Marshal.PtrToStringAnsi(out_path_ptr);
            _format_str = Marshal.PtrToStringAnsi(format_ptr);
        }
        catch
        {
            return -1;
        }

        AtlusFileSystemLibrary.FileSystems.PAK.FormatVersion _format = AtlusFileSystemLibrary.FileSystems.PAK.FormatVersion.Unknown;

        switch (_format_str)
        {
            case "v2":
                _format = AtlusFileSystemLibrary.FileSystems.PAK.FormatVersion.Version2;
                break;

            case "v2be":
                _format = AtlusFileSystemLibrary.FileSystems.PAK.FormatVersion.Version2BE;
                break;

            case "v3":
                _format = AtlusFileSystemLibrary.FileSystems.PAK.FormatVersion.Version3;
                break;

            case "v3be":
                _format = AtlusFileSystemLibrary.FileSystems.PAK.FormatVersion.Version3BE;
                break;
        }

        /* 
            V2,
        V2be,
        V3,
        V3be,*/

        {


            if (!Directory.Exists(inputPath))
            {
                return 0;
            }



            using (var pak = new PAKFileSystem(_format))
            {
                foreach (string file in Directory.EnumerateFiles(inputPath, "*.*", SearchOption.AllDirectories))
                {

                    pak.AddFile(file.Substring(inputPath.Length)
                                     .Trim(Path.DirectorySeparatorChar)
                                     .Replace("\\", "/"),
                                 file, ConflictPolicy.Ignore);
                }

                pak.Save(outputPath);
            }

            return 1;
        }
    }


}








