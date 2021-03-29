# -------------------------------------------------------------------
# Responsible for managing the test database
# usage: ManageTestDB.py [create] [clean] [<fixtures...>]
# Date: 03/27/2021
# Author: Robert Nelson
# -------------------------------------------------------------------

import datetime
import sys
import os
import pathlib
from SearchReplace import searchReplace



# Some important variables
server = "(localdb)\\MSSQLLocalDB"
create_script = 'PertDB/bin/Output/Pert_Create.sql'
project_dir = os.path.dirname(os.path.dirname(__file__))
fixtures_dir = 'PertTest/fixtures'


def run_sql(script_path, ignore_errors=False, database=None):
    if not os.path.exists(script_path):
        print('File not found at {}'.format(script_path))
        if not ignore_errors:
            sys.exit(-1)
    if database is not None:
        sql = 'sqlcmd -b -S {} -d {} -i {} -l 20\n'.format(server, database, script_path)
    else:
        sql = 'sqlcmd -b -S {} -i {} -l 20\n'.format(server, script_path)
    res = os.system(sql)
    if res != 0:
        print('An error occurred while running ' + script_path)
        if not ignore_errors:
            sys.exit(-1)
    return res


def install_fixture(fixture, database):
    path = os.path.join(os.path.join(project_dir, fixtures_dir), fixture)
    run_sql(path, database=database)


def database_exists(db):
    return os.system('sqlcmd -b -S {} -d {} -Q "SELECT * FROM dbo.[User]" -l 2\n'.format(server, db)) == 0


def main(args):
    print('ManageTestDB.py started...')

    # Find database creation script and modify it to work for our test database
    database = 'Test_SmartPertDB'
    create_script_path = os.path.join(project_dir, create_script)
    bin_dir = os.path.dirname(create_script_path)
    test_creation_script = os.path.join(bin_dir, 'Test_Pert_Create.sql')
    try:
        args.remove('create')
        should_create = True
    except ValueError:
        should_create = False
    if not os.path.exists(create_script_path):
        print(
            'Failed to find database creation script at {}, please rebuild the SmartPertDB project.'.format(create_script_path))
        sys.exit(-1)
    if not should_create:
        origin_date = datetime.datetime.fromtimestamp(pathlib.Path(create_script_path).stat().st_mtime)
        if os.path.exists(test_creation_script):
            test_date = datetime.datetime.fromtimestamp(pathlib.Path(test_creation_script).stat().st_mtime)
            should_create = test_date < origin_date
        else:
            should_create = True

    # Create database / Clean
    if should_create or not database_exists(database):
        print('Creating database ...')
        searchReplace(create_script_path, test_creation_script, 'SmartPertDB', database)
        run_sql(os.path.join(os.path.dirname(__file__), 'CleanTestDB.sql'), ignore_errors=True, database=database)
        run_sql(test_creation_script)
    elif 'clean' in args:
        print('Cleaning database ...')
        run_sql(os.path.join(os.path.dirname(__file__), 'CleanTestDB.sql'), ignore_errors=True, database=database)

    try:
        args.remove('clean')
    except ValueError:
        pass

    for arg in args:
        print(arg)
        install_fixture(arg, database)
    print('\t...Done')


if __name__ == '__main__':
    main(sys.argv[1:])
