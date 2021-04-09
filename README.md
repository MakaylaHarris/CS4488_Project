# Pert Project
SmartPert is a Pert/Gantt chart creation tool for managing projects.
Unlike other tools, this one allows for fuzzy task completion estimates which is particularly useful in software engineering.

## Client Installation
Download a [release](https://github.com/MakaylaHarris/CS4488_Project/releases) and follow the normal installation process.

## Server Installation
Installation will create the database on your target server.
SmartPert was developed using [MSSQLSERVER](https://en.wikipedia.org/wiki/Microsoft_SQL_Server)

*Prerequisite*: Setup your server.

1. Run the script Pert_Create.sql on your server. Example:
`sqlcmd -S .\MSSQLLocalDB -i Pert_Create.sql`

OR

1. Open the *SmartPertDB* Project in Visual Studio.
2. Publish the project by right clicking on *SmartPertDB* in the Solution Explorer and clicking _publish_.
3. In the Publish Database window, select the target database connection.
(ie. `(LocalDB)\\MSSQLLocalDB`)
4. Name the database *SmartPertDB*.
5. Run publish.


# Building
Building the project is done using visual studio. The solution contains the following:
* SmartPert: The main project for building the client interface in WPF.
* SmartPertDB: Project for editing and creating the database.
* PertTest: Integration testing project that uses a database set up from the SmartPertDB database build.
* Sandcastle: Builds the code documentation, found in the folder WPF/Sandcastle. Documentation is found under WPF/Help.

# Integration Tests
In order to create a clean testing environment for integration tests, a python script is used to create and clean a test database. If all tests are failing, check the following:
* Have python installed and on your system path.
* Install the pathlib module `python -m pip install pathlib`
* Build the SmartPertDB project, and ensure the script exists at `./PertDB/bin/Output/Pert_Create.sql`

## Creating Your Own Tests
Integration tests require you to set up the test database for a clean environment to work in. To do this, simply call `new TestDB();`
at the start of your test.
Fixtures can also be added either by using an existing fixture or creating your own sql script in the fixtures directory. To include the fixture when testing simply add the name of the file when creating the new test database. `new TestDB(new List<string>{'project_foo.sql'})`

## Build Configurations
* Debug: For regular debugging and testing, does not build Sandcastle documentation.
* Release: For releases, builds everything.
* Appveyor: Used by the CI Appveyor and only builds the application and test suite.

## Build Demo
For convenience a python script called *CreateDemo.py* is provided. Once the demo has been built it can be launched with the *CLICK_ME_FIRST_DEMO.bat* script, which will create a demo database and launch the demo application.
