#!/usr/bin/env python

import PIL
from PIL import Image

from argparse import ArgumentParser

from minitile import unpack_file_a2

MTS = 8

def main():
    ap = ArgumentParser()
    ap.add_argument('input_filename', nargs="+")

    ns = ap.parse_args()
    for input_filename in ns.input_filename:
        unpack_file_a2(input_filename, MTS)


if __name__ == '__main__':
    main()
