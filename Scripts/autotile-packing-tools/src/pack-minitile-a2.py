#!/usr/bin/env python

import os.path
import PIL
from PIL import Image

from argparse import ArgumentParser

from minitile import pack_file

MTS = 8

def main():
    ap = ArgumentParser()
    ap.add_argument('input_filename', nargs="+")

    ns = ap.parse_args()
    for input_filename in ns.input_filename:
        pack_file(input_filename, MTS)


if __name__ == '__main__':
    main()
