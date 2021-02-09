import sys


def searchReplace(src, dst, search, replace):
    contents = ''
    with open(src, "r") as f:
        contents = f.read()
    with open(dst, "w") as f:
        f.write(contents.replace(search, replace))


if __name__ == "__main__":
    searchReplace(sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4])
