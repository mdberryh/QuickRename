// See https://aka.ms/new-console-template for more information
using System.Text;



//TODO: should parse and confirm that the location is a valid directory and the text file is a valid file.
//  all other arguments should be treated as flags or junk
// command: RenameMovies <location> <textfile>
//if (args.Length != 3)
//{
//    Console.WriteLine("Please enter the correct number of arguments");
//    Console.WriteLine("program <location> <textfile>");
//    return 0;
//}

//var directory = args[1];
//var textNamesPath = args[2];

string directory = @"C:\Users\devman\Documents\TestRenameFiles";
string textNamesPath = @"C:\Users\devman\Documents\TestRenameFiles\names.txt";
//string[] arguments = Environment.GetCommandLineArgs();
// get the files we are going to rename.

var mediaFiles = Directory.GetFiles(directory);

// we want to exclude the text file used for the names from the files to rename.
mediaFiles = (from elem in mediaFiles where !textNamesPath.Contains(elem) select elem).ToArray();

//Read the text file that contains the new name
var newNames = File.ReadAllLines(textNamesPath);

var FileNames = from elem in newNames where !String.IsNullOrWhiteSpace(elem) select elem.Trim();

// Filter out the names of the media files that we have already renamed.
// this is likely to happen if there was a problem renaming a file and we had to start again.

// We have to use a temp list because we actually just want to check the file name not the whole path.
// if we checked the whole path the user could do something like C:/textfiles/1.txt and 
// in the text doc have: textfiles 1.txt
// using a temp array we can hold only the filenames and use that to filter names in the text doc
var tempMediaFiles = (from elem in mediaFiles select Path.GetFileNameWithoutExtension(elem));

string[] namesToChangeTo = (from elem in FileNames where !tempMediaFiles.Contains(elem) select elem).ToArray();

string[] removedNames = (from elem in FileNames where tempMediaFiles.Contains(elem) select elem).ToArray();

bool validNumberOfFiles = mediaFiles.Length == FileNames.Count();
if (!validNumberOfFiles)
{
    Console.WriteLine($"The number of media files is {mediaFiles.Length} " +
        $"and the number of names provided in text {FileNames.Count()}... " +
        $" please make sure the same number of files are in the text");
}

// Build the path to the new file name.
var fileNameArray = FileNames.ToArray();

//Have to use substring because the mediafiles for sure will have the file extension on them. 
// the files in the text document may or may not have file extensions
// we are also removing the names/paths in the directory that already match the text document
// if the script stopped partway through then we have already named somefiles. We don't want to do that again on those.
// we don't want to rename the already named files because the order in the directory has changed and may no longer match the text doc.
string[] mediaFilesToRename = (from elem in mediaFiles where !removedNames.Contains(Path.GetFileName(elem)?.Substring(0, Path.GetFileName(elem).Length - 4)) select elem)?.ToArray();

if(mediaFilesToRename.Length > 0)
{
    Console.WriteLine("Renaming files:");
}
else
{
    Console.WriteLine("There are no files to rename... quitting application");
    return 0;
}

for (int i = 0; i < mediaFilesToRename.Length; i++)
{
    // get the extention on the old one
    string ext = mediaFilesToRename[i].Substring(mediaFilesToRename[i].Length - 4, 4);

    string newName = namesToChangeTo[i];
    if (!namesToChangeTo[i].Contains('.'))
    {
        //user did not specify a file .mkv, mp4, or mov, etc...
        newName = namesToChangeTo[i] + ext;
    }

    string oldFilePath = Path.Combine(directory, mediaFilesToRename[i]);
    string newFilePath = Path.Combine(directory, newName);

    Console.WriteLine($"{oldFilePath} to {newFilePath}");
    File.Move(oldFilePath, newFilePath);
}

return 0;