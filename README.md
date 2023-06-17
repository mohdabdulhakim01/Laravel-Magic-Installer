
# 🪄Laravel Magic Installer v1.0.0

## Description : 
To install your own "copy & paste" laravel project easily and automatically configure window host file and XAMPP Apache Virtual Host file . So you can build your laravel file and quickly view it like *_**mywebsite.local**_*. 

The laravel project included are laravel version 9.19 and also included .htaccess and server.php which is very useful **for me** 😂 and convenient without accessing url through */public* folder. 

But since you are using this program. You can access all url with no problem .

    -> http://localhost/yourwebsite/your_route
    -> http://localhost/yourwebsite/public/your_route
    -> http://yourwebsite.local/your_route
    
 
## Command Flags :

    -h,--help                           -get command help
    -ip,--install-path [install_path]   -change project installation path
    -eh,--edit-host                     -edit both host file and Apache VHost together
    -i,--install [project_name]         -install new laravel project
        --hostname [hostname]           -add hostname into windows host file.
        --apache-vhost                  -add hostname to XAMPP Apache VHost file config automatically.
         --test                          -(use dummy source file to test all the command line flow.
                                         Warning* (vhost for project domain will not working properly)

Recommended Usage :

    laramin --install project_name --hostname project_name.local --apache-vhost

For Lazy Load the project domain name :

    laramin --install finaltest --hostname .local --apache-vhost

To edit both window host file and xampp vhost file :

    laramin -eh
    laramin --edit-host
This will open `notepad.exe` with host file and xampp vhost file.

## Requirement
Some command need elevated prompt ( *_**Run as Admin**_* ) such as :

    -eh, --edit-host, --hostname, and --apache-vhost
## Note :

    - The project installation might take a few minutes . This is because there are too many files for extraction.
    - If your local domain have self sign issue. You can try another domain name to make it available.
