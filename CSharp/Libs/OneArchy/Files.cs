using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Onearchy
{
    public static class Files
    {
        /// <summary>
        /// Checks to see if the given file exists
        /// </summary>
        /// <param name="path">Path including hierarchy and file</param>
        /// <returns>Returns if the file exists or not</returns>
        public static bool Exists(string path)
        {
            if (!Check(path))
            {
                throw new Exception("The given path did not contain a file");
            }

            return File.Exists(path);
        }

        /// <summary>
        /// Checks to see if the path contains a file
        /// </summary>
        /// <param name="path">Path including hierarchy and file</param>
        /// <returns>Returns if the path has an extension and thus a file</returns>
        public static bool Check(string path)
        {
            string[] hierarchy = path.Split('\\');
            string[] filecheck = hierarchy[hierarchy.Length - 1].Split('.');

            return (filecheck.Length > 1);
        }

        /// <summary>
        /// Creates a new file if it doesn't exist yet
        /// </summary>
        /// <param name="path">Path including folder and file</param>
        public static void Create(string path)
        {
            if (Path.GetExtension(path).Length > 0)
            {
                try
                {
                    string[] hierarchy = path.Split('\\');
                    string directory = path.Substring(0, (path.Length - (hierarchy[hierarchy.Length - 1].Length + 1)));

                    if (Directories.Exists(directory))
                    {
                        if (File.Exists(path))
                        {
                            throw new Exception("File already exists");
                        }
                        else
                        {
                            File.Create(path);
                        }
                    }
                    else
                    {
                        Directories.Create(directory);
                        File.Create(path);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not create file.\n" + ex.Message);
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing extension");
            }
        }

        /// <summary>
        /// Creates a file using a path and a given filename
        /// </summary>
        /// <param name="path">Path of destination folder excluding filename</param>
        /// <param name="filename">Name given to the file excluding folder hierarchy</param>
        public static void Create(string path, string filename)
        {
            if (Path.GetExtension(filename).Length > 0)
            {
                try
                {
                    string filepath = path + "\\" + filename;

                    if (Directory.Exists(path))
                    {
                        if (File.Exists(filepath))
                        {
                            throw new Exception("File already exists");
                        }
                        else
                        {
                            File.Create(filepath);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                        File.Create(filepath);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not create file.\n" + ex.Message);
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing extension");
            }
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="path">Path to the file destined to be deleted</param>
        public static void Delete(string path)
        {
            if (Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not delete file.\n" + ex.Message);
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing file");
            }
        }

        /// <summary>
        /// Copies a file from source to destination
        /// </summary>
        /// <param name="source">Path including source folder and file</param>
        /// <param name="destination">Path including destination folder and file</param>
        public static void Copy(string source, string destination)
        {
            string[] sourceHierarchy = source.Split('\\');
            string sourceExtension = Path.GetExtension(sourceHierarchy[sourceHierarchy.Length - 1]);
            string[] destinationHierarchy = source.Split('\\');
            string destinationExtension = Path.GetExtension(destinationHierarchy[destinationHierarchy.Length - 1]);

            if (sourceExtension != null && !(sourceExtension.Equals("")) && destinationExtension != null && !(destinationExtension.Equals("")))
            {
                if (Exists(source))
                {
                    if (!Exists(destination))
                    {
                        File.Copy(source, destination);
                    }
                    else
                    {
                        throw new Exception("No changes were made because of duplicate destination file");
                    }
                }
                else
                {
                    throw new Exception("No changes were made because of missing source file");
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing extension in one of the files");
            }
        }

        /// <summary>
        /// Moves a file from source to destination
        /// </summary>
        /// <param name="source">Path including source folder and file</param>
        /// <param name="destination">Path including destination folder and file</param>
        public static void Move(string source, string destination)
        {
            string[] sourceHierarchy = source.Split('\\');
            string sourceExtension = Path.GetExtension(sourceHierarchy[sourceHierarchy.Length - 1]);
            string[] destinationHierarchy = source.Split('\\');
            string destinationExtension = Path.GetExtension(destinationHierarchy[destinationHierarchy.Length - 1]);

            if (sourceExtension != null && !(sourceExtension.Equals("")) && destinationExtension != null && !(destinationExtension.Equals("")))
            {
                if (Exists(source))
                {
                    if (!Exists(destination))
                    {
                        File.Move(source, destination);
                    }
                    else
                    {
                        throw new Exception("No changes were made because of duplicate destination file");
                    }
                }
                else
                {
                    throw new Exception("No changes were made because of missing source file");
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing extension in one of the files");
            }
        }

        /// <summary>
        /// Renames a given file
        /// </summary>
        /// <param name="path">Path including folder hierarchy and old name</param>
        /// <param name="newName">The new name to give to the file indicated in the path parameter</param>
        public static void Rename(string path, string newName)
        {
            string[] hierarchy = path.Split('\\');
            string extension = Path.GetExtension(newName);
            string folder = path.Replace(hierarchy[hierarchy.Length - 1], "");

            if (extension.Length > 0)
            {
                try
                {
                    if (!Exists(path))
                    {
                        Create(folder, newName);
                    }
                    else
                    {
                        File.Move(path, (folder + newName));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not rename file.\n" + ex.Message);
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing extension");
            }
        }

        /// <summary>
        /// Replaces a given string in every file of the indicated folder
        /// </summary>
        /// <param name="path">Path to the folder in which the correction will take place</param>
        /// <param name="original">The part of the filename(s) that should be replaced</param>
        /// <param name="replace">The string which will replace the previous parameter</param>
        public static void Replace(string path, string original, string replace)
        {
            string[] files;

            try
            {
                files = Directory.GetFiles(path);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not retrieve the files of the given directory.\n" + ex.Message);
            }

            // exception handling happens in ReplaceOne
            foreach (string filepath in files)
            {
                string[] hierarchy = filepath.Split('\\');
                ReplaceOne(path + "\\" + hierarchy[hierarchy.Length - 1], original, replace);
            }
        }

        /// <summary>
        /// Replaces a given string in every path of an array of files
        /// </summary>
        /// <param name="files">The paths of all files needed to be corrected</param>
        /// <param name="original">The part of the filename(s) that should be replaced</param>
        /// <param name="replace">The string which will replace the previous parameter</param>
        public static void Replace(string[] files, string original, string replace)
        {
            // exception handling happens in ReplaceOne
            foreach (string filepath in files)
            {
                ReplaceOne(filepath, original, replace);
            }
        }

        /// <summary>
        /// Replaces a given string in a single indicated file
        /// </summary>
        /// <param name="path">Path of the file which needs to be corrected</param>
        /// <param name="original">The part of the filename that should be replaced</param>
        /// <param name="replace">The string which will replace the previous parameter</param>
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

        #region Text
        /// <summary>
        /// Returns the content of a file
        /// </summary>
        /// <param name="path">Path of the file including folder hierarchy and filename</param>
        /// <returns>Returns the content of an indicated text file.</returns>
        public static string GetContent(string path)
        {
            string content = "";

            if (Exists(path))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        content = reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not read from file.\n" + ex.Message);
                }
            }
            else
            {
                throw new Exception("File does not exist");
            }

            return content;
        }

        /// <summary>
        /// Sets the content of a file and replaces the old content
        /// </summary>
        /// <param name="path">Path of the file including folder hierarchy and filename</param>
        /// <param name="content">New content</param>
        public static void SetContent(string path, string content)
        {
            if (Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(content);
                    writer.Flush();
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing file");
            }
        }

        /// <summary>
        /// Sets the content of a file
        /// </summary>
        /// <param name="path">Path of the file including folder hierarchy and filename</param>
        /// <param name="content">New content</param>
        public static void SetContent(string path, string[] content)
        {
            if (Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    foreach (string line in content)
                    {
                        writer.WriteLine(line);
                    }

                    writer.Flush();
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing file");
            }
        }

        /// <summary>
        /// Adds content to a file
        /// </summary>
        /// <param name="path">Path of the file including folder hierarchy and filename</param>
        /// <param name="content">New additional content</param>
        public static void AddContent(string path, string content)
        {
            if (Exists(path))
            {
                string oldContent = "";

                using (StreamReader reader = new StreamReader(path))
                {
                    oldContent = reader.ReadToEnd();
                }

                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(oldContent);
                    writer.WriteLine(content);
                    writer.Flush();
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing file");
            }
        }

        /// <summary>
        /// Adds content to a file
        /// </summary>
        /// <param name="path">Path of the file including folder hierarchy and filename</param>
        /// <param name="content">New additional content</param>
        public static void AddContent(string path, string[] content)
        {
            if (Exists(path))
            {
                List<string> oldContent = new List<string>();

                using (StreamReader reader = new StreamReader(path))
                {
                    while (reader.Peek() >= 0)
                    {
                        oldContent.Add(reader.ReadLine());
                    }
                }

                using (StreamWriter writer = new StreamWriter(path))
                {
                    foreach (string line in oldContent)
                    {
                        writer.WriteLine(line);
                    }

                    foreach (string line in content)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            else
            {
                throw new Exception("No changes were made because of missing file");
            }
        }
        #endregion Text

        /// <summary>
        /// Retrieves the contents from an image file
        /// </summary>
        /// <param name="path">Path of the file including folder hierarchy and filename</param>
        /// <returns>Returns the file contents in an Image object</returns>
        public static Image GetImage(string path)
        {
            Image img;

            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        img = Image.FromStream(reader.BaseStream);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not read from file.\n" + ex.Message);
                }
            }
            else
            {
                throw new Exception("File does not exist");
            }

            return img;
        }

        /// <summary>
        /// Returns the content of a any filetype in bytes
        /// </summary>
        /// <param name="path">Path of the file including folder hierarchy and filename</param>
        /// <returns>The byte-wise content of the given file</returns>
        public static byte[] GetBinaries(string path)
        {
            byte[] data;

            if (Exists(path))
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader(path))
                    {
                        using (BinaryReader reader = new BinaryReader(streamReader.BaseStream))
                        {
                            data = reader.ReadBytes((int)reader.BaseStream.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not read from file.\n" + ex.Message);
                }
            }
            else
            {
                throw new Exception("File does not exist");
            }

            return data;
        }
    }
}