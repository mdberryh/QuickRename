// See https://aka.ms/new-console-template for more information
using System.Text;



//TODO: should parse and confirm that the location is a valid directory and the text file is a valid file.
//  all other arguments should be treated as flags or junk
// command: RenameMovies <location> <textfile>
if (args.Length != 3)
{
    Console.WriteLine("Please enter the correct number of arguments");
    Console.WriteLine("program <location> <textfile>");
    return 0;
}

var directory = args[1];
var textNamesPath = args[2];

// get the files we are going to rename.
var mediaFiles = Directory.GetFiles(directory);

// we want to exclude the text file used for the names from the files to rename.
mediaFiles = (from elem in mediaFiles where !textNamesPath.Contains(elem) select elem).ToArray();

//Read the text file that contains the new name
var newNames = File.ReadAllLines(textNamesPath);

var FileNames = from elem in newNames where !String.IsNullOrWhiteSpace(elem) select elem.Trim();

// Filter out the names of the media files that we have already renamed.
// this is likely to happen if there was a problem renaming a file and we had to start again.
string[] namesToChangeTo = (from elem in FileNames where !mediaFiles.Contains(elem) select elem).ToArray();

string[] removedNames = (from elem in FileNames where mediaFiles.Contains(elem) select elem).ToArray();

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
string[] mediaFilesToRename = (from elem in mediaFiles where !removedNames.Contains(elem.Substring(0, elem.Length - 4)) select elem).ToArray();

for (int i = 0; i < mediaFilesToRename.Length; i++)
{
    // get the extention on the old one
    string ext = mediaFiles[i].Substring(mediaFiles[i].Length - 4, 4);

    string newName = namesToChangeTo[i];
    if (!namesToChangeTo[i].Contains('.'))
    {
        //user did not specify a file .mkv, mp4, or mov, etc...
        newName = namesToChangeTo[i] + ext;
    }

    string oldFilePath = Path.Combine(directory, mediaFiles[i]);
    string newFilePath = Path.Combine(directory, newName);

    File.Move(oldFilePath, newFilePath);
}

return 0;