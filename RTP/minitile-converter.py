#!/usr/bin/env python

import os.path
import pygame.image

from argparse import ArgumentParser
from pygame.surface import Surface
from pygame.rect import Rect
from pygame.color import Color



def main():
    ap = ArgumentParser()
    ap.add_argument('input_filename', nargs="+")
    ap.add_argument("--type",
        choices=[
            "auto",
            "A1",
            "A2",
            "A3",
            "A4",
        ],
        default="auto",
    )

    ns = ap.parse_args()
    for input_filename in ns.input_filename:
        unpack_file(input_filename, ns.type)


def unpack_file(input_filename, type):

    p1, p2 = os.path.splitext(input_filename)
    output_filename = p1 + '_expanded' + p2
    
    if type == "auto":
        if 'A1' in input_filename:
            type = 'A1'
        elif 'A2' in input_filename:
            type = 'A2'
        elif 'A3' in input_filename:
            type = 'A3'
        elif 'A4' in input_filename:
            type = 'A4'
        else:
            print("Unable to detect type of file, %s" % input_filename)
            return

    insurf = pygame.image.load(input_filename)
    w, h = insurf.get_size()


    if type in ['A1', 'A2']:
        expandfunc = expand_64_96
        inblockw = 64
        inblockh = 96
        outblockw = 96
        outblockh = 160
    elif type in ['A3']:
        expandfunc = expand_64_64
        inblockw = 64
        inblockh = 64
        outblockw = 96
        outblockh = 96
    elif type in ['A4']:
        expandfunc = expand_64_160
        inblockw = 64
        inblockh = 160
        outblockw = 96
        outblockh = 256

    blocksw = w / inblockw
    blocksh = h / inblockh

    outsurf = Surface((blocksw * outblockw, blocksh * outblockh), pygame.SRCALPHA)

    for i, sx in enumerate(range(0, w, inblockw)):
        for j, sy in enumerate(range(0, h, inblockh)):
            dx = i * outblockw
            dy = j * outblockh

            inblock = insurf.subsurface(Rect(sx, sy, inblockw, inblockh))
            pygame.image.save(inblock, "/tmp/foo2.png")


            outblock = expandfunc(inblock)
            outsurf.blit(outblock, (dx, dy))

    print("Saving, " + output_filename)
    pygame.image.save(outsurf, output_filename)


def surf_swap(insurf, r1, r2):
    # Make sure the dimensions on the rects are the same
    assert r1.w == r2.w, (r1, r2)
    assert r1.h == r2.h, (r1, r2)

    # Copy surface at rect1 to new surface
    tmp1 = Surface((r1.w, r1.h), pygame.SRCALPHA)
    tmp1.blit(insurf, (0, 0), r1)

    # Overwrite insurf's r1 with r2
    insurf.fill(Color(0, 0, 0, 0), r1)
    insurf.blit(insurf, r1.topleft, r2)

    # Overwrite insurf's r2 with tmp1
    insurf.fill(Color(0, 0, 0, 0), r2)
    insurf.blit(tmp1, r2.topleft, Rect(0, 0, r1.w, r1.h))
    #pygame.image.save(tmp1, '/tmp/foo3.png')


def expand_64_96(source):
    dest = Surface((96, 160), pygame.SRCALPHA)

    # Row 1
    dest.blit(source, (0, 0), Rect((0, 32), (32, 32)))
    dest.blit(source, (32, 0), Rect((16, 32), (32, 32)))
    surf_swap(dest, Rect(32, 0, 16, 32), Rect(48, 0, 16, 32))
    dest.blit(source, (64, 0), Rect((32, 32), (32, 32)))

    # Row 2
    dest.blit(source, (0, 32), Rect((0, 48), (32, 32)))
    surf_swap(dest, Rect(0, 32, 32, 16), Rect(0, 48, 32, 16))
    dest.blit(source, (32, 32), Rect((16, 48), (32, 32)))
    surf_swap(dest, Rect(32, 32, 16, 32), Rect(48, 32, 16, 32))
    surf_swap(dest, Rect(32, 32, 32, 16), Rect(32, 48, 32, 16))
    dest.blit(source, (64, 32), Rect((32, 48), (32, 32)))
    surf_swap(dest, Rect(64, 32, 32, 16), Rect(64, 48, 32, 16))


    # Row 3
    dest.blit(source, (0, 64), Rect((0, 64), (32, 32)))
    dest.blit(source, (32, 64), Rect((16, 64), (32, 32)))
    surf_swap(dest, Rect(32, 64, 16, 32), Rect(48, 64, 16, 32))
    dest.blit(source, (64, 64), Rect((32, 64), (32, 32)))

    # Row 4, Col 1
    dest.blit(source, (0, 96), Rect((16, 48), (32, 32)))
    dest.fill(Color(0, 0, 0, 0), Rect((16, 112), (16, 16)))
    dest.blit(source, (16, 112), Rect((48, 16), (16, 16)))

    # Row 4, Col 2
    dest.blit(source, (32, 96), Rect((16, 48), (32, 32)))
    dest.fill(Color(0, 0, 0, 0), Rect((32, 112), (16, 16)))
    dest.blit(source, (32, 112), Rect((32, 16), (16, 16)))

    # Row 5, Col 1
    dest.blit(source, (0, 128), Rect((16, 48), (32, 32)))
    dest.fill(Color(0, 0, 0, 0), Rect((16, 128), (16, 16)))
    dest.blit(source, (16, 128), Rect((48, 0), (16, 16)))

    # Row 5, Col 2
    dest.blit(source, (32, 128), Rect((16, 48), (32, 32)))
    dest.fill(Color(0, 0, 0, 0), Rect((32, 128), (16, 16)))
    dest.blit(source, (32, 128), Rect((32, 0), (16, 16)))

    # Row 4, Col 3, (diagonals)
    dest.blit(source, (64, 96), Rect((16, 48), (32, 32)))
    dest.fill(Color(0, 0, 0, 0), Rect((80, 112), (16, 16)))
    dest.blit(source, (80, 112), Rect((48, 16), (16, 16)))
    dest.fill(Color(0, 0, 0, 0), Rect((64, 96), (16, 16)))
    dest.blit(source, (64, 96), Rect((32, 0), (16, 16)))

    # Row 5, Col 3, (alt. diagonals)
    dest.blit(source, (64, 128), Rect((16, 48), (32, 32)))

    dest.fill(Color(0, 0, 0, 0), Rect((64, 144), (16, 16)))
    dest.blit(source, (64, 144), Rect((32, 16), (16, 16)))
    dest.fill(Color(0, 0, 0, 0), Rect((80, 128), (16, 16)))
    dest.blit(source, (80, 128), Rect((48, 0), (16, 16)))

    return dest


def expand_64_64(source):
    dest = Surface((96, 96), pygame.SRCALPHA)

    # Row 1
    dest.blit(source, (0, 0), Rect((0, 0), (32, 32)))
    dest.blit(source, (32, 0), Rect((16, 0), (32, 32)))
    surf_swap(dest, Rect(32, 0, 16, 32), Rect(48, 0, 16, 32))
    dest.blit(source, (64, 0), Rect((32, 0), (32, 32)))

    # Row 2
    dest.blit(source, (0, 32), Rect((0, 16), (32, 32)))
    surf_swap(dest, Rect(0, 32, 32, 16), Rect(0, 48, 32, 16))
    dest.blit(source, (32, 32), Rect((16, 16), (32, 32)))
    surf_swap(dest, Rect(32, 32, 16, 32), Rect(48, 32, 16, 32))
    surf_swap(dest, Rect(32, 32, 32, 16), Rect(32, 48, 32, 16))
    dest.blit(source, (64, 32), Rect((32, 16), (32, 32)))
    surf_swap(dest, Rect(64, 32, 32, 16), Rect(64, 48, 32, 16))

    # Row 3
    dest.blit(source, (0, 64), Rect((0, 32), (32, 32)))
    dest.blit(source, (32, 64), Rect((16, 32), (32, 32)))
    surf_swap(dest, Rect(32, 64, 16, 32), Rect(48, 64, 16, 32))
    dest.blit(source, (64, 64), Rect((32, 32), (32, 32)))

    return dest

def expand_64_160(source):
    dest = Surface((96, 256), pygame.SRCALPHA)

    A2_input = source.subsurface(Rect(0, 0, 64, 96))
    A2_output = expand_64_96(A2_input)

    A3_input = source.subsurface(Rect(0, 96, 64, 64))
    A3_output = expand_64_64(A3_input)

    dest.blit(A2_output, (0, 0), Rect((0, 0), (96, 160)))
    dest.blit(A3_output, (0, 160), Rect((0, 0), (96, 96)))

    return dest


if __name__ == '__main__':
    main()
