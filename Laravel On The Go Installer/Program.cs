using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Security.Principal;
namespace Laravel_Magic_installer
{
    class Program
    {
        static string app_version()
        {
            return "1.0.0";
        }

        static string hostfile()
        {
            //string current_app_path = System.IO.Directory.GetCurrentDirectory();
            //string hostfile = current_app_path + "\\resources\\hosts";
            string hostfile = @"c:\Windows\System32\Drivers\etc\hosts";
            return hostfile;
        }
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string current_app_path = System.IO.Directory.GetCurrentDirectory();
            string config_file_path = current_app_path + "\\laramin-config.json";
            JObject configObj = JObject.Parse(File.ReadAllText(config_file_path));
            string install_path = (string)configObj[@"install_path"];
            
            if (args.Count() == 0) { helpCommand(); return; }
            int index = 0;
            Dictionary<string, string> cmdDict = new Dictionary<string, string>();
            string[] command_list = {"-l","-h","-i","-ip","--list","--help","--install","--install-path","--test","--hostname","--apache-vhost"};
            string[] install_command_list = { "-i","--install", "--test", "--hostname","--apache-vhost" };
            string[] exception_cmd_list = {"--test","--apache-vhost"};
            foreach (string arg in args)
            {
                if (arg == "-h" || arg == "--help") { helpCommand(); return; }
                if (arg == "-ip" || arg == "--install-path") { installPath(args,configObj,config_file_path); return; }
                if (arg == "-eh" || arg == "--edit-host") { openHostEditor(); return; }
                // special combo command
                // -install , --host ,--test

                if (install_command_list.Contains(arg))
                {
                    string getValue = cmdArgFind(args, index);
                    if (command_list.Contains(getValue) && !exception_cmd_list.Contains(arg) ) // if collison between flag happen for example --install --hostname .then raise error. except some command like --test and --apache-vhost which is non value based flag
                    {
                        helpCommand(); return;
                    }
                    else
                    {
                     //   Console.WriteLine("DEV:Command : " + arg + " value : " + getValue);
                        // change arg to shortform
                        string shortFormValue = getCmdShortForm(arg);
                        cmdDict.Add(shortFormValue, getValue);
                    }
                }
              
                index++;
                
            }
            if (args.Contains("-i") || args.Contains("--install"))
            {

                installProject(install_path, cmdDict);
            }
            else
            {
                Console.WriteLine("Error : Incorrect Flag, use --help or -h for command details.");

            }
           
            Console.ResetColor();
            



        }
        static void helpCommand()
        {
            Console.WriteLine("Laravel Magic Installer v" + app_version());
            Console.WriteLine("Help :\n"

                            + "    -h,--help\t\t\t\t-get command help\n"
                            + "    -ip,--install-path [install_path]\t-change project installation path\n"
                            + "    -eh,--edit-host\t\t\t-edit both host file and Apache VHost together\n"
                            + "    -i,--install [project_name]\t\t-install new laravel project\n"
                            + "    \t--hostname [hostname]\t\t-add hostname into windows host file. \n"
                            + "    \t--apache-vhost\t\t\t-add hostname to XAMPP Apache VHost file config automatically.\n"
                            + "    \t--test\t\t\t\t-(use dummy source file to test all the command line flow. "
                            + "     \t\t\t\t\t\t Warning* (vhost for project domain will not working properly) \n\n"
                            + "Recommended Usage :\n"
                            + "    laramin --install project_name --hostname project_name.local --apache-vhost\n\n"
                            + "For Lazy Load the project domain name :\n"
                            + "    laramin --install finaltest --hostname .local --apache-vhost\n\n"
                            + "To edit both window host file and xampp vhost file :\n"
                            + "    laramin -eh \n"
                            + "    laramin --edit-host \n\n"
                            + "Note :\n"
                            + "    * The project installation might take a few minutes . This is because there are too many files for extraction.\n"
                            + "    * If your local domain have self sign issue. You can try another domain name to make it available."
                            );
        }
        static void installProject(string install_path,Dictionary<string,string> cmdDict)
        {
            string current_app_path = System.IO.Directory.GetCurrentDirectory();
            string install_resource =current_app_path+ @"\resources\safeinstall.zip";
            string project_path = install_path;
            bool proceed_install = false;
            string app_loading_text = "Laravel Magic Installer v" + app_version() + "\n\n" + "Installing Laravel Project \"" + cmdDict["-i"] + "\". . .";
            bool proceed_hostname_change = false;
            bool proceed_apache_vhost_change = false;
            foreach (var elem in cmdDict)
            {
                //Console.WriteLine("DEV : Shortform Key : " + elem.Key + " Value :  " + elem.Value);
                if (elem.Key == "-i")
                {
                    project_path += "\\"+ elem.Value;
                    if (elem.Value == "")
                    {
                        Console.WriteLine("Project name must be included.");
                        return;
                    }

                    if (Directory.Exists(project_path))
                    {
                        Console.WriteLine("The project already exist .");
                        return;
                    }

                    proceed_install = true;
                    

                }
                if(elem.Key =="-t"){
                    install_resource = current_app_path + @"\resources\safeinstall_test.zip";
                }
                if (elem.Key == "-hn")
                {
                    if (isAdmin())
                    {

                        proceed_hostname_change = true;
                    }else{
                        Console.WriteLine("Error : Changing hostname failed. Please run this program as administrator !");
                        return;
                    }
                }
                if (elem.Key == "-avh")
                {
                    if (proceed_hostname_change)
                    {
                        proceed_apache_vhost_change = true;
                    }
                    else
                    {
                        Console.WriteLine("Error: Apache VHost will not be added due since --hostname flag didn't trigger.");
                        return;
                    }
                }

            }
            if (proceed_install)
            {
                Console.WriteLine(app_loading_text);
                Console.Write("Extracting the zip. This might take a 3-4 minutes. . .");
                //// extracting zip file

                Directory.CreateDirectory(project_path);
                System.IO.Compression.ZipFile.ExtractToDirectory(install_resource, project_path);

                ////////

                // adding localhost name
                if (proceed_hostname_change) { changeHostname(cmdDict["-hn"],cmdDict["-i"], project_path,proceed_apache_vhost_change); }
                
                Console.WriteLine("\r\nDone. You are ready to go !.");
                Thread.Sleep(500);
                System.Diagnostics.Process.Start("explorer.exe ", project_path);
            }
            else
            {
                Console.WriteLine("Error : Any supporting command such as --test or --hostname must including --install first !");
                helpCommand();
                return;
            }


        }
        static bool isAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            return isAdmin;
        }
     
        static void installPath(string[] args,JObject configObj,string config_file_path){
             if (args.Count() ==1)
                    {
                        Console.WriteLine("Install Path : "+(string)configObj[@"install_path"]);
                    }
                    else
                    {
                        configObj[@"install_path"] = args[1];
                        File.WriteAllText(config_file_path, configObj.ToString());
                        Console.WriteLine("New install path configured : " + args[1]);
                    }
                    return;
        }
        static string cmdArgFind(string[] arguments,int indexNumber)
        {
          
          //  Console.WriteLine("DEV:arg count : " + (arguments.Count()-1) + " indexNumber : " + indexNumber);
            if(arguments.Count()-1 < indexNumber+1){
                return "";
            }
            return arguments[indexNumber + 1];
        }
        static string getCmdShortForm(string argument){
            string shortform = "";
            if(argument.Length >2){
                switch(argument){
                    case "--install":
                        shortform =  "-i";
                        break;
                    case "--test":
                       shortform =  "-t";
                        break;
                    case "--hostname":
                       shortform =  "-hn";
                        break;
                    case "--apache-vhost":
                        shortform = "-avh";
                        break;
                }
            }
            return shortform;
        }
        static void changeHostname(string hostname,string project_name,string project_path,bool apachevhostflag)
        {
            
            string hostfile_data = File.ReadAllText(hostfile());
            string laramin_ref_host_start = "# [HOST FILE EDIT BY LARAMIN]:START";
            string laramin_ref_host_end = "# [HOST FILE EDIT BY LARAMIN]:END";
            Dictionary<string, string> host_pointer = new Dictionary<string, string>();
            string ip_address_local = "127.0.0";
            string old_laramin_ref_host_data = "";
            string new_laramin_ref_host_data = "";
            string new_ip_address = "";
            string lazy_loaded_hostname = "";
            if (hostname[0] == '.')
            { // name shortcut method. automatically use project name as domain name with extension
                hostname = project_name + hostname;
                lazy_loaded_hostname = "lazy loaded";
            }

            if (hostfile_data.Contains(laramin_ref_host_start)) // check if laramin footprint exist to append new 
            {
                // grab laramin reference host content
                string contentLine = betweenStrings(hostfile_data, laramin_ref_host_start, laramin_ref_host_end);
                string[] contentArray = contentLine.Split(
                            new[] { "\r\n", "\r", "\n" },
                            StringSplitOptions.None
                        );
                int new_ip_incr = 100;
                int inc_content = 0;
                bool isReplacable = false;
                foreach (string per_line in contentArray)
                {
                    
                    if (per_line != "")
                    {
                        string[] per_elem = per_line.Split(' ');
                        host_pointer.Add(per_elem[0], per_elem[1]);
                        new_ip_incr++;
                        if (inc_content == contentArray.Count() - 1)
                        {
                            old_laramin_ref_host_data += per_line;
                        }
                        else
                        {
                            old_laramin_ref_host_data += per_line + "\r\n";
                        }
                        isReplacable = true;

                    }


                }
                if (isReplacable)
                {
                    new_ip_address = ip_address_local + "." + new_ip_incr.ToString();
                    host_pointer.Add(new_ip_address, hostname);
                    int inc_pointer = 0;
                    foreach (var elem in host_pointer)
                    {
                        // Console.WriteLine("IP: "+elem.Key+" Domain : "+elem.Value);
                        if (inc_pointer == host_pointer.Count() - 1)
                        {
                            new_laramin_ref_host_data += elem.Key + " " + elem.Value;
                        }
                        else
                        {
                            new_laramin_ref_host_data += elem.Key + " " + elem.Value + "\r\n";
                        }

                    }
                    hostfile_data = hostfile_data.Replace(old_laramin_ref_host_data, new_laramin_ref_host_data);
                }
                else
                {
                    int new_ip_incr_ = 100;
                    new_ip_address = ip_address_local + "." + new_ip_incr_.ToString();
                    hostfile_data += "\r\n\r\n" + laramin_ref_host_start + "\r\n" + new_ip_address + " " + hostname + "\r\n" + laramin_ref_host_end;
                }
                
            }
            else
            {
                int new_ip_incr = 100;
                new_ip_address = ip_address_local + "." + new_ip_incr.ToString();
                hostfile_data += "\r\n\r\n"+laramin_ref_host_start + "\r\n" + new_ip_address + " " + hostname + "\r\n" + laramin_ref_host_end;

            }
           
            File.WriteAllText(hostfile(), hostfile_data);
            Console.Write("\r\nHostname added ( ");
            consoleSpecialColor(hostname);
            Console.Write( " ) - "+ lazy_loaded_hostname+"\r\n");
            if (apachevhostflag)
            {
                changeApacheVHost(hostname, new_ip_address, project_path);
            }
        }

        static void changeApacheVHost(string hostname,string ip_address,string project_path)
        {
            Console.Write("XAMPP Apache VHost (");
             consoleSpecialColor(hostname);
             Console.Write(") added.\r\n");
            string vhost_file_path = @"C:\xampp\apache\conf\extra\httpd-vhosts.conf";
            string vhost_file_data = File.ReadAllText(vhost_file_path);
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black ;
            Console.WriteLine("#Hint : Make sure to enable vhost setting in XAMPP httpd.conf file ( first time only )");
            Console.WriteLine("#Hint : Restart the server to make sure domain is online.");
            string vhost_setting = "\r\n<VirtualHost "+ip_address+":80>\r\n"+
	                                "DocumentRoot \""+project_path+"\\public\"\r\n"+
	                                "DirectoryIndex index.php\r\n"+
	                                "ServerName "+hostname+"\r\n"+
                                    "<Directory \"" + project_path + "\">\r\n" +
		                            "    Options Indexes FollowSymLinks MultiViews\r\n"+
		                            "    AllowOverride all\r\n"+
		                            "    Order Deny,Allow\r\n"+
		                            "    Allow from all\r\n"+
		                            "    Require all granted\r\n"+
	                                "</Directory>\r\n"+
                                    "</VirtualHost>\r\n";
            vhost_file_data += vhost_setting;
            File.WriteAllText(vhost_file_path, vhost_file_data);
            Console.ResetColor();
        }

        public static String betweenStrings(String text, String start, String end)
        {
            int p1 = text.IndexOf(start) + start.Length;
            int p2 = text.IndexOf(end, p1);

            if (end == "") return (text.Substring(p1));
            else return text.Substring(p1, p2 - p1);
        }
        public static void consoleSpecialColor(string value)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write(value);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void openHostEditor()
        {
            if (!isAdmin())
            {
                Console.WriteLine("Error : Executing Flag \"-eh\". Please run this app as administrator !");
                return;
            }
            string hostfile_path = @"c:\Windows\System32\Drivers\etc\hosts";
            string xampp_apache_vhost_path = @"C:\xampp\apache\conf\extra\httpd-vhosts.conf";
            System.Diagnostics.Process.Start("notepad.exe ", hostfile_path);
            System.Diagnostics.Process.Start("notepad.exe ", xampp_apache_vhost_path);

        }
        
    }
}
