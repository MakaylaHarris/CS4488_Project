import sys


def searchReplace(src, dst, search, replace=""):
    with open(src, "r") as f:
        contents = f.read()
    with open(dst, "w") as f:
        f.write(contents.replace(search, replace))


def searchReplaceFileToText(src, search, replace=""):
    with open(src, "r") as f:
        contents = f.read()
        return contents.replace(search, replace)

if __name__ == "__main__":
    if len(sys.argv) < 4:
        print("Insufficient number of arguments")
        sys.exit(-1)
    replace = "" if len(sys.argv) == 4 else sys.argv[4]
    searchReplace(sys.argv[1], sys.argv[2], sys.argv[3], replace)
