using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;

namespace ClipboardManager
{
    internal class Program
    {
        static string helpText = "Clipboard Manager for windows files\r\n" +
                                 "\r\n" +
                                 "Usage\r\n" +
                                 $"\t$ {AppDomain.CurrentDomain.FriendlyName} [--copy [-s]] [--cut] [--paste [-f]] [--show] [--clear] <files>\r\n" +
                                 "\r\n" +
                                 "Examples\r\n" +
                                 $"\t$ {AppDomain.CurrentDomain.FriendlyName} --copy test1.txt test2.txt\r\n" +
                                 $"\t$ {AppDomain.CurrentDomain.FriendlyName} --copy -s \"hello\"\r\n" +
                                 $"\t$ {AppDomain.CurrentDomain.FriendlyName} --paste\r\n";

        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0 || args[0] == "--help")
                {
                    Console.WriteLine(helpText);
                    return 0;
                }

                // Some magic bytes for the cut command, this way you can also paste with ctrl + v
                byte[] moveEffect = new byte[] { 2, 0, 0, 0 };
                MemoryStream dropEffect = new MemoryStream();
                dropEffect.Write(moveEffect, 0, moveEffect.Length);

                string command = args[0];
                if (command == "--clear")
                {
                    Clipboard.Clear();
                }
                else if (command == "--show")
                {
                    var paths = Clipboard.GetFileDropList();
                    if (paths.Count == 0)
                    {
                        var text = Clipboard.GetText();
                        if (!string.IsNullOrEmpty(text))
                        {
                            Console.WriteLine(text);
                        }
                    }
                    else
                    {
                        foreach (var path in paths)
                        {
                            Console.WriteLine(path);
                        }
                    }
                }
                else if (command == "--copy")
                {
                    bool copyString = false;
                    if (args.Length > 1 && args[1] == "-s")
                    {
                        copyString = true;
                    }

                    if (args.Length < 2 + (copyString ? 1 : 0))
                    {
                        Console.WriteLine("At least one item needs to be specified");
                        return -1;
                    }

                    StringCollection items = new StringCollection();
                    for (int i = 1 + (copyString ? 1 : 0); i < args.Length; i++)
                    {
                        items.Add(Path.GetFullPath(args[i]));
                    }

                    Clipboard.Clear();

                    if (copyString)
                    {
                        Clipboard.SetText(string.Join("\r\n", items.Cast<string>()));
                    }
                    else
                    {
                        Clipboard.SetFileDropList(items);
                    }
                }
                else if (command == "--cut")
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("At least one path needs to be specified");
                        return -1;
                    }

                    DataObject data = new DataObject();
                    StringCollection paths = new StringCollection();
                    for (int i = 1; i < args.Length; i++)
                    {
                        paths.Add(Path.GetFullPath(args[i]));
                    }

                    data.SetFileDropList(paths);
                    data.SetData("Preferred DropEffect", dropEffect);

                    Clipboard.Clear();
                    Clipboard.SetDataObject(data, true);
                }
                else if (command == "--paste")
                {
                    bool force = false;
                    int currentArgumentIndex = 1;
                    if (args.Length > currentArgumentIndex && args[currentArgumentIndex] == "-f")
                    {
                        force = true;
                        currentArgumentIndex++;
                    }

                    string destination = Directory.GetCurrentDirectory();
                    if (args.Length > currentArgumentIndex)
                    {
                        destination = args[currentArgumentIndex];
                    }

                    bool isCutted = false;

                    // Check the magic bytes to see if the file is cutted
                    var data = Clipboard.GetDataObject();
                    if (data != null &&
                        data.GetData("Preferred DropEffect") is MemoryStream setDropEffect &&
                        setDropEffect != null &&
                        setDropEffect.ToArray().SequenceEqual(dropEffect.ToArray()))
                    {
                        isCutted = true;
                    }

                    var paths = Clipboard.GetFileDropList();
                    foreach (var path in paths)
                    {
                        string destinationFileName = Path.Combine(destination, Path.GetFileName(path));

                        var sourceFile = new FileInfo(path);
                        if (isCutted)
                        {
                            // MoveTo doesn't have overwrite flag so we manually delete
                            if (force && File.Exists(destinationFileName))
                            {
                                var destFileInfo = new FileInfo(destinationFileName);
                                destFileInfo.Delete();
                            }

                            sourceFile.MoveTo(destinationFileName);
                        }
                        else
                        {
                            sourceFile.CopyTo(destinationFileName, force);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Unrecognized command: {command}");
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }

            return 0;
        }
    }
}
