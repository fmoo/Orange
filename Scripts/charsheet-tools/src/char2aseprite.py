"""Opens an RPGMaker sprite subsheet and writes it to an .aseprite file"""

from argparse import ArgumentParser
from pathlib import Path
import PIL
from PIL import Image
from io import BytesIO


def split_file(input_filename):
    inp = Path(input_filename)
    im = Image.open(input_filename)
    imw, imh = im.size
    cw, ch = imw / 3, imh / 4
    assert cw == int(
        cw
    ), f"{input_filename} width ({imw}) is not an even multiple of 3."
    assert ch == int(
        ch
    ), f"{input_filename} height ({imh}) is not an even multiple of 4."

    nn = 0
    for ii in range(2):
        for jj in range(4):
            subimage = im.crop((jj * cw, ii * ch, (jj + 1) * cw, (ii + 1) * ch))

            nn += 1


def has_image_data(image):
    imw, imh = image.size
    lastpixel = None
    for ii in range(imh):
        for jj in range(imw):
            px = image.getpixel((jj, ii))
            if lastpixel is None:
                lastpixel = px
            else:
                if lastpixel != px:
                    return True
    return False


def main():
    ap = ArgumentParser()
    ap.add_argument("input_filename", nargs="+")

    ns = ap.parse_args()

    for fn in ns.input_filename:
        split_file(fn)


if __name__ == "__main__":
    main()
