using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Onearchy
{
    public static class Directories
    {
        /// <summary>
        /// Checks to see if the directory exists
        /// </summary>
        /// <param name="path">Path of the directory including hierarchy</param>
        /// <returns>Returns if the directory exists or not</returns>
        public static bool Exists(string path)
        {
            if (!Check(path))
            {
                throw new Exception("The last item in the path hierarchy is no directory");
            }

            return Directory.Exists(path);
        }

        /// <summary>
        /// Checks to see if the last item in the path is a directory
        /// </summary>
        /// <param name="path">Path including hierarchy</param>
        /// <returns>Returns if the path has no extension and thus if it is a directory</returns>
        public static bool Check(string path)
        {
            string[] hierarchy = path.Split('\\');
            string[] filecheck = hierarchy[hierarchy.Length - 1].Split('.');

            return (filecheck.Length < 2);
        }

        public static void Create(string path)
        {
            if (Check(path))
            {
                if (!Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    throw new Exception("The folder already exists");
                }
            }
            else
            {
                throw new Exception("Target folder path was a file");
            }
        }

        public static void Delete(string path, bool content = false)
        {
            if (content)
            {

            }
            else
            {
                if (Content(path).Length >= 1)
                {
                    throw new Exception("Could not delete file because it contains files and/or folders");
                }
            }
        }

        public static void Copy(string source, string destination)
        {
            if (Exists(source))
            {
                if (!Exists(destination))
                {
                    //Directory.Copy(source, destination); bestaat nog niet
                }
                else
                {
                    throw new Exception("No changes were made because of duplicate destination folder");
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing source folder");
            }
        }

        public static void Move(string source, string destination)
        {
            if (Exists(source))
            {
                if (!Exists(destination))
                {
                    Directory.Move(source, destination);
                }
                else
                {
                    throw new Exception("No changes were made because of duplicate destination folder");
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing source folder");
            }
        }

        public static void Rename(string path, string newName)
        {
            string[] hierarchy = path.Split('\\');
            string folder = path.Replace(hierarchy[hierarchy.Length - 1], newName);

            try
            {
                if (!Exists(path))
                {
                    Create(folder);
                }
                else
                {
                    Move(path, (folder + newName));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not rename folder.\n" + ex.Message);
            }
        }

        public static void Replace(string path, string original, string replace)
        {
            string[] folders;

            try
            {
                folders = Folders(path);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not retrieve the files of the given directory.\n" + ex.Message);
            }

            // exception handling happens in ReplaceOne
            foreach (string folderpath in folders)
            {
                string[] hierarchy = folderpath.Split('\\');
                ReplaceOne(path + "\\" + hierarchy[hierarchy.Length - 1], original, replace);
            }
        }

        public static void Replace(string[] folders, string original, string replace)
        {
            // exception handling happens in ReplaceOne
            foreach (string folderpath in folders)
            {
                ReplaceOne(folderpath, original, replace);
            }
        }

        public static void ReplaceOne(string path, string original, string replace)
        {
            string newName = "";

            try
            {
                string[] hierarchy = path.Split('\\');
                newName = hierarchy[hierarchy.Length - 1].Replace(original, replace);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not replace a section of the filename.\n" + ex.Message);
            }

            // this one has it's own inner exception handling
            Rename(path, newName);
        }

        public static string[] Files(string path, bool subfiles = true)
        {
            string[] files;
            DirectoryInfo directory = new DirectoryInfo(path);

            if (subfiles)
            {

            }
            else
            {
                FileInfo[] fileInfo = directory.GetFiles();
                files = new string[fileInfo.Length];
            }
        }

        public static string[] Folders(string path, bool subfolders = true)
        {
            if (subfolders)
            {

            }
            else
            {

            }
        }

        public static string[] Content(string path, bool subcontent = true)
        {
            if (subcontent)
            {

            }
            else
            {

            }
        }

        /// <summary>
        /// Returns the base folder of a given path
        /// </summary>
        /// <param name="path">Path of the directory including hierarchy</param>
        /// <returns>Returns the path of the directory enveloping the given directory</returns>
        public static string BaseFolder(string path)
        {
            string[] hierarchy = path.Split('\\');
            string basepath = "";

            if (hierarchy.Length < 2)
            {
                throw new Exception("The path did not contain a base folder to return");
            }

            foreach (string folder in hierarchy)
            {
                if (!folder.Equals(hierarchy[hierarchy.Length - 1]))
                {
                    basepath += folder + "\\";
                }
                else
                {
                    basepath += folder;
                }
            }

            return basepath;
        }
    }
}
