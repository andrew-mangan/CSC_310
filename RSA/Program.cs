using System;
using System.IO;
using System.Linq;

class PhotoOrganizer
{
    static void Main()
    {
        Console.WriteLine("This will sort all common photos and videos formats into month and year folders.");
        Console.WriteLine("Enter the path to the media folder:");
        string sourcePath = Console.ReadLine();

        
        if (!Directory.Exists(sourcePath))
        {
            Console.WriteLine("Directory does not exist.");
            return;
        }

        /*Console.WriteLine("Do you want to sort photos (1) or Videos (2)? (Default:Photos) 1 or 2 ");
        string input = Console.ReadLine();

        */
        string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".mp4", ".avi", ".heic", ".mov", ".mkv" };


        var imageFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)
            .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
            .ToList();

        Console.WriteLine($"Found {imageFiles.Count} image files.");

        foreach (var file in imageFiles)
        {
            try
            {
                DateTime modifiedDate = File.GetLastWriteTime(file);
                string Yfolder = modifiedDate.Year.ToString();
                string Mfolder = modifiedDate.Month.ToString("D2");
                //string folderName = modifiedDate.ToString("yyyy-MM");
                string targetFolder = Path.Combine(sourcePath, Yfolder, Mfolder);

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                string fileName = Path.GetFileName(file);
                string destinationPath = Path.Combine(targetFolder, fileName);

                // Avoid overwriting: if file exists, append number
                int counter = 1;
                string originalName = Path.GetFileNameWithoutExtension(file);
                string extension = Path.GetExtension(file);

                while (File.Exists(destinationPath))
                {
                    destinationPath = Path.Combine(targetFolder, $"{originalName}_{counter}{extension}");
                    counter++;
                }

                File.Move(file, destinationPath);
                Console.WriteLine($"Moved: {file} → {destinationPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file '{file}': {ex.Message}");
            }
        }

        Console.WriteLine("Media organization complete.");
        Console.WriteLine("Do you wish to purge empty folders?  Yes/No ");
        string purge = Console.ReadLine();
        
        
        if (purge.ToLower() == "yes" || purge.ToLower() == "y")
        {

            int deletedCount = DeleteEmptyFolders(sourcePath);
            Console.WriteLine($"Scan complete. Deleted {deletedCount} empty folders.");
        }
        else
        {
            Console.WriteLine("No purge attempted.");
            return;
        }
 

    }

    static int DeleteEmptyFolders(string path)
    {
        int deletedFolders = 0;

        foreach (var directory in Directory.GetDirectories(path))
        {
            // Recursively clean subdirectories first
            deletedFolders += DeleteEmptyFolders(directory);

            // After cleaning, check if the current folder is now empty
            if (!Directory.EnumerateFileSystemEntries(directory).Any())
            {
                try
                {
                    Directory.Delete(directory);
                    Console.WriteLine($"Deleted: {directory}");
                    deletedFolders++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete {directory}: {ex.Message}");
                }
            }
        }

        return deletedFolders;
    }
}