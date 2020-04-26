from argparse import ArgumentParser
from pathlib import Path
import PIL
import sys
from PIL import Image
from typing import Any


def cmd_split(ns: Any) -> int:
    input_filenames = ns.input_filename
    for input_filename in input_filenames:
        inp = Path(input_filename)
        im = Image.open(input_filename)
        imw, imh = im.size
        cw, ch = imw / 12, imh / 8
        assert cw == int(
            cw
        ), f"{input_filename} width ({imw}) is not an even multiple of 12."
        assert ch == int(
            ch
        ), f"{input_filename} height ({imh}) is not an even multiple of 8."

        nn = 0
        for ii in range(2):
            for jj in range(4):
                subimage = im.crop(
                    (jj * cw * 3, ii * ch * 4, (jj + 1) * cw * 3, (ii + 1) * ch * 4)
                )
                subp = inp.with_name(f"{inp.stem}-split{nn}{inp.suffix}")
                print(f"Writing {subp}...")
                subimage.save(subp.name)
                nn += 1
    return 0


def cmd_makestrip(ns: Any) -> int:
    for fn in ns.input_filename:
        inp = Path(fn)
        inim = Image.open(fn)
        outim = stripify(inim, inp.name, ns)
        outp = inp.with_name(f"{inp.stem}-{ns.suffix}.png")
        print(f"Writing {outp}...")
        outim.save(outp)
    return 0


DIRECTION_ROW = {"R": 2, "U": 3, "L": 1, "D": 0}


def stripify(input_image: Image, name: str, ns: Any):
    imw, imh = input_image.size
    cw, ch = imw / 3, imh / 4
    assert cw == int(cw), f"image {name} width ({imw}) is not an even multiple of 3."
    assert ch == int(ch), f"image {name} height ({imh}) is not an even multiple of 4."
    cw = int(cw)
    ch = int(ch)

    STRIP_ORDER = []
    for rowcode in ns.direction_order:
        for framecode in ns.frame_order:
            STRIP_ORDER.append((DIRECTION_ROW[rowcode], int(framecode)))

    output_image = Image.new("RGBA", (cw * len(STRIP_ORDER), ch))

    nn = 0
    for ii, jj in STRIP_ORDER:
        subimage = input_image.crop((jj * cw, ii * ch, (jj + 1) * cw, (ii + 1) * ch))
        output_image.paste(subimage, (nn * cw, 0))
        nn += 1

    return output_image


def main():
    ap = ArgumentParser()
    sp = ap.add_subparsers()

    cmd = sp.add_parser("split")
    cmd.add_argument("input_filename", nargs="+")
    cmd.set_defaults(cmd=cmd_split)

    cmd = sp.add_parser("makestrip")
    cmd.add_argument("--direction-order", default="RULD")
    cmd.add_argument("--frame-order", default="1012")
    cmd.add_argument("--suffix", default="strip")
    cmd.add_argument("input_filename", nargs="+")
    cmd.set_defaults(cmd=cmd_makestrip)

    ns = ap.parse_args()
    return ns.cmd(ns)


if __name__ == "__main__":
    sys.exit(main())
