# This is a sample Python script.
# ----------------------------------------------------------
# Script is used to create a demo of SmartPert
# Created 2/8/2021 by Robert Nelson
# ----------------------------------------------------------
import os
import shutil
import sys
from SearchReplace import searchReplace
from which import which

# -------------------------------------------------------------------
# Functions
# -------------------------------------------------------------------

def genString():
    exe_path = os.path.join(demo_folder, exe_name)
    return (
        f':: Auto-Generated Script to create database on {server} and run SmartPert\n'
        f'@echo off\n'
        f'sqlcmd -b -S {server} -i {demo_folder}/Clean.sql\n'
        f'echo Trying to connect to server...\n'
        f'sqlcmd -b -S {server} -i {demo_folder}/Create.sql -l 30\n'
        f'IF NOT ERRORLEVEL 0'
        f' sqlcmd -b -S {server} -i {demo_folder}/Create.sql\n'
        f'sqlcmd -S {server} -i {demo_folder}/Insert.sql\n'
        f'{exe_path}\n'
    )


###############################################################################
# SETUP SECTION
###############################################################################
zip = False
for arg in sys.argv:
    if arg.endswith("zip"):
        zip = True
print("Create Demo started...")

# Some important variables
base_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
os.chdir(base_dir)
server = "(localdb)\\MSSQLLocalDB"
sql_create = os.path.join(base_dir, "PertDB/bin/Demo/Pert_Create.sql")
sql_insert = os.path.join(base_dir, "PertDB/Scripts/Insert.sql")
db_proj = "PertDB/SmartPertDB.sqlproj"
demo_folder = os.path.join(base_dir, "Demo")
exe_name = "SmartPert.exe"
search = "SmartPertDB"
replace = "SPDB_Demo"
prj_base = "WPF"
bin_dir = os.path.join(base_dir, "WPF/bin/Demo")
smartpert_prj = os.path.join(prj_base, 'SmartPert.csproj')
apps_file = os.path.join(prj_base, 'App.config')
startup = os.path.join(prj_base, "CLICK_ME_FIRST_DEMO.bat")
msbuild = 'msbuild'
if not which(msbuild):
    print('Unable to find msbuild, please add it to your path environment')
    sys.exit(-1)


if not os.path.exists(smartpert_prj):
    print(f"Unable to find {smartpert_prj}! Are you in the root directory and built Demo configuration?")
    sys.exit(-1)

# Change the app.config connection string to the demo database
searchReplace(apps_file, apps_file, search, replace)

# -------------------------------------------------------------------
#   BUILD
# -------------------------------------------------------------------
print('Building...')
# SmartPert Database
os.system(msbuild + ' -property:Configuration=Release -property:OutDir=bin/Demo ' + db_proj)
# Main app
os.system(msbuild + ' -property:Configuration=Release -property:OutDir=bin/Demo ' + smartpert_prj)


# -------------------------------------------------------------------
# Post build
# -------------------------------------------------------------------
print("Cleaning up...")
# Fix the app settings properties back up
searchReplace(apps_file, apps_file, replace, search)
# Clean up anything old
if os.path.exists(demo_folder):
    shutil.rmtree(demo_folder)
if not os.path.exists(demo_folder):
    os.mkdir(demo_folder)

# First, gather all the required files
print(f"Copying program files from {bin_dir}...")
files = os.listdir(bin_dir)
for file in files:
    fpath = os.path.join(bin_dir, file)
    if not os.path.isdir(fpath):
        shutil.copy(fpath, demo_folder)

if not os.path.exists(os.path.join(demo_folder, exe_name)):
    print(f'Failed to copy {exe_name}')
    sys.exit(-1)

print("Copying Database Scripts...")
searchReplace(sql_create, os.path.join(demo_folder, 'Create.sql'), search, replace)
searchReplace(sql_insert, os.path.join(demo_folder, 'Insert.sql'), search, replace)
searchReplace(os.path.join(os.path.dirname(__file__), 'CleanTestDB.sql'), os.path.join(demo_folder, 'clean.sql'), 'Test_SmartPertDB', replace)

# Now Make start up script
print(f"Creating {startup} Script...")

with open(startup, "w") as f:
    f.write(genString())

# Zip the files?
if(zip):
    from Package import zip_all
    zip_file = os.path.join(base_dir, 'DemoAndCode.zip')
    if not which('7z'):
        print('Unable to find 7z on your path, zipping files failed!')
    else:
        print('Creating zip file...')
        if os.path.exists(zip_file):
            os.remove(zip_file)
        cmd = f"git archive -o code.zip HEAD"
        if(os.system(cmd)):
            print('Failed to create archive of code files!')
            sys.exit(-1)
        if(zip_all(zip_file, files=['code.zip', 'Demo/', 'CLICK_ME_FIRST_DEMO.bat'])):
            print('Failed to Add demo files!')
            sys.exit(-1)


print('done!')
