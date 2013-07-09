uDebate_Discussion
==================


The discussion tree section of uDebate is the third of three modules comprising the uDebate module developed in the context of Puzzled by Policy

This is a DNN module and should be installed under a valid DNN (v.6 or higher) installation.

The database script udebate.sql should be executed prior to module installation

The thee modules that must be installed are:

uDebate uDebateThreads uDebate_Discussion

Follow this steps to install the module:

1. Install DNN 6.XX  (latest version here https://dotnetnuke.codeplex.com/releases/view/104373 )
2. Add the following lines in your web.config inside the <appSettings> section:
   
   <!-- Settings for uDebate -->
    <add key="ConnectionString" value="Data Source=.\SQLExpress;AttachDbFilename=|DataDirectory|Database.mdf;Integrated Security=True;User Instance=True;persist security info=True;" />  
    <add key="hostServer" value="http://localhost/dnn627" />
    <add key="DomainName" value="http://localhost/dnn627" />
    <!-- end uDebate --> 

   You must adjust the 'value' attributes to match your connection string and DomainName

3. Run udebate.sql (You can use 'Host > SQL' to access the database from within the portal)
4. Install the modules using the install zip files found in the packages folder of each module.
5. Run testthread.sql to create a new test thread
6. Create three pages named: udebate, udebatethreads,udebatediscussion and add the respective modules to each one
7. Set the parmissions to all pages to be visible to 'All Users' via Pages -> Page settings -> Permissions from the Admin menu
8. Hide pages udebatethreads and udebatediscussion from the menu. The navigation to these will be only made via the main udebate page.This setting is under Pages -> Page settings -> Page Details -> Include in Menu 

