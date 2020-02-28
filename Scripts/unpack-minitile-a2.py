#!/usr/bin/env python

import os.path
import PIL
from PIL import Image

from argparse import ArgumentParser


def main():
    ap = ArgumentParser()
    ap.add_argument('input_filename', nargs="+")

    ns = ap.parse_args()
    for input_filename in ns.input_filename:
        unpack_file(input_filename)


def unpack_file(input_filename):
    p1, p2 = os.path.splitext(input_filename)
    output_filename = p1 + '_expanded' + p2
    input_image = Image.open(input_filename)
    imw, imh = input_image.size
    INW, INH = (32, 48)
    OUTW, OUTH = (80, 48)

    rows, rwrem = divmod(imh, INH)
    cols, clrem = divmod(imw, INW)

    assert rwrem == 0, f'Invalid Height: ({imh})'
    assert clrem == 0, f'Invalid Width: ({imw})'

    outimw, outimh = cols * OUTW, rows * OUTH
    output_image = Image.new("RGBA", (outimw, outimh))

    for ii in range(rows):
        for jj in range(cols):
            subimage = input_image.crop(
                (jj * INW, ii * INH, (jj+1) * INW, (ii+1) * INH))
            converted = unpack_subimage(subimage)
            # converted.save(f'{p1}.{ii}.{jj}.png')
            output_image.paste(converted, (jj * OUTW, ii * OUTH))
            # subimage.save(f'{p1}.{ii}.{jj}.png')

    # print((rows, cols))
    output_image.save(output_filename)

# left, upper, right, lower


def unpack_subimage(image):
    OUTW, OUTH = (80, 48)
    output_image = Image.new("RGBA", (OUTW, OUTH))

    def blit(src, src_bounds, dst, dst_offs):
        dst.paste(src.crop((src_bounds)), dst_offs)
    blit(image, (0, 16, 32, 48), output_image, (0, 0))
    blit(output_image, (0, 16, 32, 32), output_image, (0, 32))
    blit(output_image, (16, 0, 32, 48), output_image, (32, 0))
    blit(output_image, (0, 8, 48, 16), output_image, (0, 24))
    blit(output_image, (0, 32, 48, 40), output_image, (0, 16))
    blit(output_image, (32, 0, 40, 48), output_image, (16, 0))
    blit(output_image, (8, 0, 16, 48), output_image, (24, 0))

    blit(output_image, (16, 16, 32, 32), output_image, (48, 0))
    blit(output_image, (16, 16, 32, 32), output_image, (64, 0))
    blit(output_image, (16, 16, 32, 32), output_image, (48, 16))
    blit(output_image, (16, 16, 32, 32), output_image, (64, 16))
    blit(output_image, (16, 16, 32, 32), output_image, (48, 32))
    blit(output_image, (16, 16, 32, 32), output_image, (64, 32))

    blit(image, (16, 0, 24, 8), output_image, (48, 0))  # UL
    blit(image, (24, 8, 32, 16), output_image, (72, 24))  # BR
    blit(image, (24, 0, 32, 8), output_image, (72, 0))   # UR
    blit(image, (16, 8, 24, 16), output_image, (48, 24))  # BL

    blit(image, (16, 0, 24, 8), output_image, (48, 32))  # UL
    blit(image, (24, 8, 32, 16), output_image, (56, 40))  # BR
    blit(image, (24, 0, 32, 8), output_image, (72, 32))   # UR
    blit(image, (16, 8, 24, 16), output_image, (64, 40))  # BL

    return output_image


if __name__ == '__main__':
    main()
