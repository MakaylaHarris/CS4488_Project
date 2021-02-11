import os

def which(pgm):
    path = os.getenv('PATH')
    for p in path.split(os.path.pathsep):
        p = os.path.join(p, pgm)
        if os.path.exists(p) and os.access(p, os.X_OK):
            return p
        p += '.exe'
        if os.path.exists(p) and os.access(p, os.X_OK):
            return p


if __name__ == '__main__':
    if len(sys.argv) < 2 or not which(sys.argv[1]):
        sys.exit(-1)
