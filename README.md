# Laravel-Magic-Installer
 Automatically Install Laravel Project with host file configuration

Laravel Magic Installer v1.0.0
Help :
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

Note :
    * The project installation might take a few minutes . This is because there are too many files for extraction.
    * If your local domain have self sign issue. You can try another domain name to make it available.
