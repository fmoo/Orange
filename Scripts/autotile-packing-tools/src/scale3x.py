#!/usr/bin/env python

import os.path
import PIL
from PIL import Image

from argparse import ArgumentParser

from minitile import upscale_file

def main():
    ap = ArgumentParser()
    ap.add_argument('input_filename', nargs="+")

    ns = ap.parse_args()
    for input_filename in ns.input_filename:
        upscale_file(input_filename, 3)


if __name__ == '__main__':
    main()