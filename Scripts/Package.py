##############################################
#   Packages up binaries for Release
#       Created 2/10/2021 by Robert Nelson
###############################################
import os
from which import which

def check_programs(programs):
    for p in programs:
        if not which(p):
            return p;

def zip_all(dest, files=[]):
    return os.system(f"7z a {dest} " + ' '.join(files))

if __name__ == '__main__':
    missing = check_programs(['7z'])
    if missing:
        print(f'Unable to find {missing} on your system path!')
        sys.exit(-1)

    # Check if we're in root folder
    if os.path.basename(os.getcwd()) == "Scripts":
        os.chdir("..")
    output = 'SmartPert.zip'
    dir = os.path.abspath(os.getcwd())
    zip_dir = os.path.join(dir, output)

    # Clean up
    if os.path.exists(zip_dir):
        os.remove(zip_dir)


    print(f'Creating Zip {output}')
    # Create zip
    zip_all(zip_dir, files=['README.md'])
    os.chdir(os.path.join(dir, 'WPF/bin/Release'))
    zip_all(zip_dir, files=['./'])
    os.chdir(os.path.join(dir, 'PertDB/bin/Output'))
    zip_all(zip_dir, files=['Pert_Create.sql'])
    print('\t...done')
