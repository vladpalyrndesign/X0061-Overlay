using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;



namespace VladUtils
{
    public class FilesUtils
    {
        public static Boolean DeleteFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return true;
                else
                {
                    File.Delete(path);
                    if (!File.Exists(path))
                        return true;
                    else
                        return false;
                }
            }
            catch (SystemException e)
            {
                return false;
            }
        }

        public static Boolean CopyFile(string sourceFileName, string destinationFileName)
        {
            try
            {
                if (!File.Exists(sourceFileName))
                    return false;
                else
                {
                    if (DeleteFile(destinationFileName))
                        File.Copy(sourceFileName, destinationFileName);
                    else
                        return false;
                    if (File.Exists(destinationFileName))
                        return true;
                    else
                        return false;
                }
            }
            catch (SystemException e)
            {
                return false;
            }
        }

        public static void Message(string message)
        {
            System.Windows.Forms.MessageBox.Show(message);
        }
        public static List<string> readFileContent(string path)
        {
            List<string> retval = new List<string>();
            if (File.Exists(path))
            {
                retval = File.ReadAllLines(path).ToList();
            }
            return retval;
        }
        public static List<string> ReadCleanFileContent(string path)
        {
            List<string> retval = readFileContent(path);
            retval.RemoveAll(q => q.StartsWith("#") || q.Trim() == "");
            return retval;
        }
        public static string GetPathOfExecutedDLL()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        public static string GetDefaultBrowserPath()
        {
            string browser = string.Empty;
            //RegistryKey key = null;

            //try
            //{
            //    // try location of default browser path in XP
            //  //  key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

            //    // try location of default browser path in Vista
            //    if (key == null)
            //    {
            //        key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http", false); ;
            //    }

            //    if (key != null)
            //    {
            //        //trim off quotes
            //        browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
            //        if (!browser.EndsWith("exe"))
            //        {
            //            //get rid of everything after the ".exe"
            //            browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
            //        }

            //        key.Close();
            //    }
            //}
            //catch
            //{
            //    return string.Empty;
            //}

            //return browser;
            string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            string browserPathKey = @"$BROWSER$\shell\open\command";

            RegistryKey userChoiceKey = null;
            string browserPath = "";

            try
            {
                //Read default browser path from userChoiceLKey
                userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                //If user choice was not found, try machine default
                if (userChoiceKey == null)
                {
                    //Read default browser path from Win XP registry key
                    var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                    //If browser path wasn’t found, try Win Vista (and newer) registry key
                    if (browserKey == null)
                    {
                        browserKey =
                        Registry.CurrentUser.OpenSubKey(
                        urlAssociation, false);
                    }
                    //var path = CleanifyBrowserPath(browserKey.GetValue(null) as string);
                    var path = browserKey.GetValue(null) as string;
                    browserKey.Close();
                    return path;
                }
                else
                {
                    // user defined browser choice was found
                    string progId = (userChoiceKey.GetValue("ProgId").ToString());
                    userChoiceKey.Close();

                    // now look up the path of the executable
                    string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                    var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                    browserPath = kp.GetValue(null) as string;
                    kp.Close();
                    browserPath = browserPath.Substring(0, browserPath.IndexOf(".exe") + 5);

                    return browserPath;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string GetFileFullPath(string extension, string initPath = "", string typeofile = "", string message = "")
        {
            
            System.Windows.Forms.OpenFileDialog dialogGetFullFilePath = new System.Windows.Forms.OpenFileDialog();
            if (initPath != "") dialogGetFullFilePath.InitialDirectory = initPath;
            dialogGetFullFilePath.Filter = typeofile + " (*." + extension + ")|*." + extension + "|All files (*.*)|*.*";
            dialogGetFullFilePath.FilterIndex = 2;
            dialogGetFullFilePath.RestoreDirectory = true;

            if (dialogGetFullFilePath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return (File.Exists(dialogGetFullFilePath.FileName)? dialogGetFullFilePath.FileName:"");
            }
            return "";
        }


        public static string SaveFileDialogBox(string extension, string initPath = "", string typeofile = "", string message = "")
        {

            System.Windows.Forms.SaveFileDialog dialogGetFullFilePath = new System.Windows.Forms.SaveFileDialog();
            dialogGetFullFilePath.Title = message;
            if (initPath != "")
            {
                dialogGetFullFilePath.InitialDirectory =Path.GetDirectoryName( initPath);
                dialogGetFullFilePath.FileName = initPath;
                //dialogGetFullFilePath.se


            }
            dialogGetFullFilePath.Filter = typeofile + " (*." + extension + ")|*." + extension + "|All files (*.*)|*.*";
           // dialogGetFullFilePath.FilterIndex = 2;
           // dialogGetFullFilePath.RestoreDirectory = true;

            if (dialogGetFullFilePath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dialogGetFullFilePath.FileName;
            }
            return "";
        }


        public static string OpenFileDialogBox(string extension, string initPath = "", string typeofile = "", string message = "")
        {

            System.Windows.Forms.OpenFileDialog dialogGetFullFilePath = new System.Windows.Forms.OpenFileDialog();
            dialogGetFullFilePath.Title = message;

            if (initPath != "")
            {
                dialogGetFullFilePath.InitialDirectory = Path.GetDirectoryName(initPath);
                dialogGetFullFilePath.FileName = initPath;
            }
            dialogGetFullFilePath.Filter = typeofile + " (*." + extension + ")|*." + extension + "|All files (*.*)|*.*";
            // dialogGetFullFilePath.FilterIndex = 2;
            // dialogGetFullFilePath.RestoreDirectory = true;

            if (dialogGetFullFilePath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dialogGetFullFilePath.FileName;
            }
            return "";
        }
        public static Boolean Protocol(string FileName, string text)
        {
            try
            {
                File.AppendAllText(FileName, "\n" + text);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Extract number from path 
        /// "d:\acadrn projects\0235\0369\0384\Administration\Courier Transmittals\2004\0384-july-23-04c(eden oak).doc" 
        /// retrun 0235
        /// </summary>
        /// <param name="SourceFile"></param>
        /// <returns></returns>
        public static string GetFirstNumberFromPath(string SourceFile)
        {
            List<string> splittedPath = SourceFile.Split('\\').ToList();
            int number = 0;
            splittedPath.RemoveAll(q => !Int32.TryParse(q, out number));
            if (splittedPath.Count > 0)
                return splittedPath[0];
            else
                return "";
        }
        //public static List<string> ReadBlocksNameWhereGradientAndSolidHatchWillNotBeDelete ()
        //{

        //    string path = "";
        //    path = GetPathOfExecutedDLL() +
        //        "\\" +
        //        Kapow.Properties.Resources.DoNotDeleteSolidHatchForTheseBlocks.ToString();

        //    List<string> fileContent =
        //        new List<string>();
        //    fileContent =
        //        ReadCleanFileContent(path);

        //    return fileContent;
        //}
        //public static List<string> GetLayersSates()
        //{

        //    string path = "";
        //    path = GetPathOfExecutedDLL() +
        //        "\\" +
        //        Kapow.Properties.Resources.LayersStates.ToString();

        //    List<string> fileContent =
        //        new List<string>();
        //    fileContent =
        //        ReadCleanFileContent(path);

        //    return fileContent;
        //}


    }
}
