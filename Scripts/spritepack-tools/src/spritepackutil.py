from argparse import ArgumentParser
from aseprite import extract_frame, extract_images, dump_file
from pathlib import Path
import PIL
import sys
import re
import easygui

from PIL import Image
from typing import Any, List, Tuple, NamedTuple, Optional

def cmd_split(ns: Any) -> int:
    input_filenames = ns.input_filename
    nccols = ns.cols * ns.subcols
    ncrows = ns.rows * ns.subrows

    for input_filename in input_filenames:
        inp = Path(input_filename)
        im = Image.open(input_filename)
        imw, imh = im.size
        cw, ch = imw / nccols, imh / ncrows
        assert cw == int(
            cw
        ), f"{input_filename} width ({imw}) is not an even multiple of {nccols}."
        assert ch == int(
            ch
        ), f"{input_filename} height ({imh}) is not an even multiple of {ncrows}."

        nn = 0
        for ii in range(2):
            for jj in range(4):
                subimage = im.crop(
                    (
                        jj * cw * ns.subcols,
                        ii * ch * ns.subrows,
                        (jj + 1) * cw * ns.subcols,
                        (ii + 1) * ch * ns.subrows,
                    )
                )
                subp = inp.with_name(f"{inp.stem}-split{nn}{inp.suffix}")
                print(f"Writing {subp}...")
                subimage.save(subp.name)
                nn += 1
    return 0


def common_prefix(names: List[str]) -> str:
    prefix = ""
    if len(names) == 0:
        return prefix
    elif len(names) == 1:
        return names[0]

    done = False
    for ii, wantletter in enumerate(names[0]):
        for other in names[1:]:
            if ii >= len(other):
                done = True
                break
            if wantletter != other[ii]:
                done = True
                break
        if not done:
            prefix += wantletter

    return prefix


def cmd_join(ns: Any) -> int:
    input_filenames = ns.input_filename
    chunksize = ns.rows * ns.cols
    if ns.sort:
        print("Using sorted input file list...")
        input_filenames.sort()
    nchunk = 0
    for ii in range(0, len(input_filenames), chunksize):
        root = Path(input_filenames[0]).parent
        chunk_names = input_filenames[ii : ii + chunksize]
        prefix = common_prefix([Path(fn).name for fn in chunk_names])
        chunk_images = [Image.open(filename) for filename in chunk_names]
        cursize = None
        for image in chunk_images:
            if cursize is None:
                cursize = image.size
            elif cursize != image.size:
                print(
                    f"Error: All images must have the same size.  Saw both {cursize} and {image.size}"
                )
                filelist = "\n - ".join(input_filenames[ii : ii + chunksize])
                print(f"with files: \n - {filelist}")
                return 1
        if cursize is None:
            raise Exception()

        outimage = join_images(chunk_images, cursize, ns.rows, ns.cols)
        if prefix == "":
            name = f"joined{nchunk}"
        else:
            name = f"{prefix}-joined{nchunk}.png"
        print(f"prefix = {prefix}")
        print(f"Writing {name}...")
        outimage.save(root / name)

        nchunk += 1
    return 0


def join_images(
    images: List[Image.Image], cellsize: Tuple[int, int], numrows: int, numcols: int
) -> Image:
    cw, ch = cellsize
    imw, imh = cw * numcols, ch * numrows
    output_image = Image.new("RGBA", (imw, imh))
    for nn, image in enumerate(images):
        row, col = divmod(nn, 3)
        output_image.paste(image, (col * cw, row * ch))
    return output_image


def cmd_makestrip(ns: Any) -> int:
    for fn in ns.input_filename:
        inp = Path(fn)
        inim = Image.open(fn)
        outim = stripify(inim, inp.name, ns)
        outp = inp.with_name(f"{inp.stem}-{ns.suffix}.png")
        print(f"Writing {outp}...")
        outim.save(outp)
    return 0


def cmd_aseprite_extract(ns: Any) -> int:
    for fn in ns.input_filename:
        extract_images(fn)
    return 0


a2s_token_matcher = re.compile(r'f(\d+)r(\d+)c(\d+)(f?)');

class SheetToken(NamedTuple):
    frame: int
    row: int
    col: int
    flip: bool


def cmd_aseprite_makesheet(ns: Any) -> int:
    tokens: List[SheetToken] = []
    files: List[str] = []
    for token in ns.token_or_filename:
        match = a2s_token_matcher.match(token)
        if match is not None:
            tokens.append(SheetToken(int(match.group(1)), int(match.group(2)), int(match.group(3)), True if match.group(4) else False))
        else:
            files.append(token)

    rows: int = max(tokens, key=lambda t: t.row).row
    cols: int = max(tokens, key=lambda t: t.col).col

    print(f'Using rows={rows} cols={cols}')

    for input_filename in files:
        output: Optional[Image.Image] = None
        for token in tokens:
            frame = extract_frame(input_filename, token.frame)
            if output is None:
                output = Image.new("RGBA", (cols * frame.size[0], rows * frame.size[1]))

            if token.flip:
                frame = frame.transpose(Image.FLIP_LEFT_RIGHT)

            output.paste(frame, ((token.col - 1) * frame.size[0], (token.row - 1) * frame.size[1]))

        outpath = None
        inp = Path(input_filename)
        if ns.prompt:
            outpath = easygui.enterbox("Enter Filename", "Save As...", f"{inp.stem}.png")
            if outpath is None:
                print("No output filename specified. Aborting.")
                return -1
            outpath = inp.with_name(outpath)

        else:
            outpath = inp.with_name(
                f"{inp.stem}.png"
            )

        if output is None:
            print("No tokens to extract.")
        else:
            print(f"Writing {outpath}...")
            output.save(outpath)

    return 0


def cmd_aseprite_info(ns: Any) -> int:
    for fn in ns.input_filename:
        dump_file(fn)
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
    cmd.add_argument("--rows", type=int)
    cmd.add_argument("--cols", type=int)
    cmd.add_argument(
        "--subrows",
        help="number of subrows (for validation; default: 1)",
        default=1,
        type=int,
    )
    cmd.add_argument(
        "--subcols",
        help="number of subcolumns (for validation; default: 1)",
        default=1,
        type=int,
    )
    cmd.set_defaults(cmd=cmd_split)

    cmd = sp.add_parser("join")
    cmd.add_argument("input_filename", nargs="+")
    cmd.add_argument("--rows", type=int)
    cmd.add_argument("--cols", type=int)
    cmd.add_argument("--sort", type=bool, default=True)
    cmd.set_defaults(cmd=cmd_join)

    cmd = sp.add_parser("makestrip")
    cmd.add_argument("--direction-order", default="RULD")
    cmd.add_argument("--frame-order", default="1012")
    cmd.add_argument("--suffix", default="strip")
    cmd.add_argument("input_filename", nargs="+")
    cmd.set_defaults(cmd=cmd_makestrip)

    cmd = sp.add_parser("aseprite-extract")
    cmd.add_argument("input_filename", nargs="+")
    cmd.set_defaults(cmd=cmd_aseprite_extract)

    cmd = sp.add_parser("aseprite-makesheet")
    cmd.add_argument("token_or_filename", nargs="+")
    cmd.add_argument("--prompt", action="store_true")
    cmd.set_defaults(cmd=cmd_aseprite_makesheet)

    cmd = sp.add_parser("aseprite-info")
    cmd.add_argument("input_filename", nargs="+")
    cmd.set_defaults(cmd=cmd_aseprite_info)

    ns = ap.parse_args()
    return ns.cmd(ns)


if __name__ == "__main__":
    sys.exit(main())
