
using System.CommandLine;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.ComponentModel;


var bundleOption = new Option<FileInfo>("--output", "File path name");
bundleOption.AddAlias("-o");
var bundleLanguage = new Option<string>("--language", "File extension ") { IsRequired = true };
bundleLanguage.AddAlias("-l");
var bundleNote = new Option<bool>("--note", "add file path");
bundleNote.AddAlias("-n");
var bundleSort = new Option<string>("--sort", "how to sort");
bundleSort.AddAlias("-s");
var bundleRemove = new Option<bool>("--remove-empty-lines", "empty lines will be removed");
bundleRemove.AddAlias("-r-e-l");
var bundleAuthor = new Option<string>("--author", "the file's author");
bundleAuthor.AddAlias("-a");

var bundleCommand = new Command("bundle", "Bundles code files to a single file");
var rspCommand = new Command("create-rsp", "response file for bundle command");

string[] arrayOfFileTypes = { ".py", ".css", ".cs", ".sql", ".json", ".txt", ".html", ".js", ".java", ".bat" };


bundleCommand.AddOption(bundleOption);
bundleCommand.AddOption(bundleLanguage);
bundleCommand.AddOption(bundleNote);
bundleCommand.AddOption(bundleSort);
bundleCommand.AddOption(bundleRemove);
bundleCommand.AddOption(bundleAuthor);

bundleCommand.SetHandler((output, language, note, sort, remove, author) =>
{

    string bundleContent = "";


    if (author != null)
        bundleContent += ("Author: " + author + "\n\n");

    else
        bundleContent += "";

    try
    {
        var files = Directory.GetFiles(Directory.GetCurrentDirectory());

        if (sort.Equals("type"))
            Array.Sort(files, (x, y) => Path.GetExtension(x).CompareTo(Path.GetExtension(y)));//sorting according to type

        else  //default ab
            Array.Sort(files);

        if (language.Equals("all"))
        {
            foreach (var file in files)
            {
                bool readableType = Array.Exists(arrayOfFileTypes, y => y == Path.GetExtension(file));

                if (readableType && !file.Equals(output.FullName))
                {

                    if (note)
                        bundleContent += ("Path: " + file + "\n");

                    string[] content = File.ReadAllLines(file);
                    string[] goodContent = File.ReadAllLines(file);
                    int i = 0;
                    if (remove)
                    {

                        for (int j = 0; j < content.Length; j++)
                        {


                            if (!content[j].Equals(""))
                            {
                                goodContent[i++] = content[j];

                            }
                        }
                    }
                    else
                        i = content.Length;

                    for (int j = 0; j < i; j++)
                        bundleContent += (goodContent[j] + "\n");
                  
                    bundleContent += "\n\n"; //for a clearer view
                }
            }
        }
        else
        {
            foreach (var file in files)
            {
                bool readableType = Array.Exists(arrayOfFileTypes, y => y == Path.GetExtension(file));
                if (Path.GetExtension(file).Equals('.' + language) && readableType && !file.Equals(output.FullName))
                {
                    if (note)
                        bundleContent += (file + "\n");
                    string[] content = File.ReadAllLines(file);
                    string[] goodContent = File.ReadAllLines(file);
                    int i = 0;
                    if (remove)
                    {
                        for (int j = 0; j < content.Length; j++)
                        {
                            if (!content[j].Equals(""))
                            {
                                goodContent[i++] = content[j];
                            }
                        }
                    }
                    else
                        i = content.Length;

                    for (int j = 0; j < i; j++)
                    {
                        bundleContent += (goodContent[j] + "\n");
                    }
                    bundleContent += "\n\n";//for a clearer view
                }
            }
        }
        File.WriteAllText(output.FullName + ".txt", bundleContent);
        Console.WriteLine("The bundle file was made successfully!");
    }
    catch (DirectoryNotFoundException ex)
    {


        Console.WriteLine("Error: File path is invalid");
    }


}, bundleOption, bundleLanguage, bundleNote, bundleSort, bundleRemove, bundleAuthor);


rspCommand.SetHandler(() =>
{
    Console.WriteLine("Please name your response file, no need to add the extension");
    string rsp = Console.ReadLine();
    Console.WriteLine("Name your bundle file ");
    string name = Console.ReadLine();
    Console.WriteLine("Please enter the files full path, by entering \"this\" your path will be the current path");
    string path = Console.ReadLine();
    if (path.Equals("this"))
        path = System.IO.Directory.GetCurrentDirectory();
    Console.WriteLine("Enter the readable language, by entering \"all\" you will receive all the languages");
    string lang = Console.ReadLine();
    Console.WriteLine("Would you like all the files' paths added to your bundle file?  ( y/n )");
    string temp = Console.ReadLine();
    bool note = false;
    if (temp.Equals("y"))
        note = true;
    Console.WriteLine("How do you want the order of the files to be? alphabetical  order (\"ab\") or order by file extensions (\"type\") ? (default is ab)");
    string order = Console.ReadLine();
    Console.WriteLine("Do you want to remove the empty lines? (y/n)");
    temp = Console.ReadLine();
    bool remove = false;
    if (temp.Equals("y"))
        remove = true;
    Console.WriteLine("do you want to add an author to show who the file was created by? ( y/n )");
    temp = Console.ReadLine();
    bool author = false;
    string authorName="";

    if (temp.Equals("y"))
    {
        author = true;
        Console.WriteLine("please snter the author's name:");
        authorName = Console.ReadLine();
    }


    string all = "bundle --output \"" + path + "\\" + name + "\" --language " + lang + " --note " + note + " --sort " + order + " --remove-empty-lines " + remove;
    if (author)
        all += (" --author \"" + authorName+"\"");
    try
    {
        string p = System.IO.Directory.GetCurrentDirectory();
        File.WriteAllText(p + "\\" + rsp + ".rsp", all);

        Console.WriteLine("To run the bundle command please write: fib @" + rsp + ".rsp ");
    }
    catch (DirectoryNotFoundException ex)
    {
        Console.WriteLine("the path you put in is invalid");
    }
});


var rootCommand = new RootCommand("Root command for file Bundler CLI");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(rspCommand);

rootCommand.InvokeAsync(args);

