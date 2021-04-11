import os
import os.path
import PIL
import sys
from PIL import Image
from argparse import ArgumentParser
from typing import Any


def upscale_file(input_filename, factor):
    p1, p2 = os.path.splitext(input_filename)
    output_filename = f"{p1}_{factor}x{p2}"
    input_image = Image.open(input_filename)
    imw, imh = input_image.size
    output_image = input_image.resize(
        (int(imw * factor), int(imh * factor)), Image.NEAREST
    )
    output_image.save(output_filename)


def pack_file(input_filename, MTS=8):
    p1, p2 = os.path.splitext(input_filename)
    output_filename = p1 + "_packed" + p2
    input_image = Image.open(input_filename)
    imw, imh = input_image.size
    INW, INH = (MTS * 10, MTS * 6)
    OUTW, OUTH = (MTS * 4, MTS * 6)

    rows, rwrem = divmod(imh, INH)
    cols, clrem = divmod(imw, INW)

    assert rwrem == 0, f"Invalid Height: ({imh})"
    assert clrem == 0, f"Invalid Width: ({imw})"

    outimw, outimh = cols * OUTW, rows * OUTH
    output_image = Image.new("RGBA", (outimw, outimh))

    for ii in range(rows):
        for jj in range(cols):
            subimage = input_image.crop(
                (jj * INW, ii * INH, (jj + 1) * INW, (ii + 1) * INH)
            )
            converted = pack_subimage(subimage, MTS)
            output_image.paste(converted, (jj * OUTW, ii * OUTH))

    output_image.save(output_filename)


def flatten_tiled_file_to_gm16(input_image, MTS=8, zigzag=False):
    imw, imh = input_image.size
    nw, nh = int(imw / MTS / 2 / 5), int(imh / MTS / 2 / 3)
    numcells = nw * nh

    output_image = Image.new("RGBA", (16 * (MTS * 2), (1 + numcells) * (MTS * 2)))
    INW = MTS * 2 * 5
    INH = MTS * 2 * 3

    nn = 1
    for ii in range(nh):
        if zigzag and ii % 2 == 1: continue
        for jj in range(nw):
            input_subimage = input_image.crop(
                (jj * INW, ii * INH, (jj + 1) * INW, (ii + 1) * INH)
            )
            converted = flatten_tiled_subimage_to_gm16(input_subimage)
            output_image.paste(converted, (0, nn * 2 * MTS))
            nn += 1

            if zigzag:
                input_subimage = input_image.crop(
                    (jj * INW, (ii+1) * INH, (jj + 1) * INW, (ii + 2) * INH)
                )
                converted = flatten_tiled_subimage_to_gm16(input_subimage)
                output_image.paste(converted, (0, nn * 2 * MTS))
                nn += 1

    return output_image


def flatten_tiled_subimage_to_gm16(image, MTS=8):
    output_image = Image.new("RGBA", (16 * (MTS * 2), (MTS * 2)))

    blit(image, (MTS * 2, MTS * 2, MTS * 4, MTS * 4),
         output_image, (MTS * 0, MTS * 0))
    blit(image, (MTS * 6, MTS * 0, MTS * 8, MTS * 2),
         output_image, (MTS * 2, MTS * 0))
    blit(image, (MTS * 8, MTS * 0, MTS * 10, MTS * 2),
         output_image, (MTS * 4, MTS * 0))
    blit(image, (MTS * 2, MTS * 0, MTS * 4, MTS * 2),
         output_image, (MTS * 6, MTS * 0))
    blit(image, (MTS * 6, MTS * 2, MTS * 8, MTS * 4),
         output_image, (MTS * 8, MTS * 0))
    blit(image, (MTS * 0, MTS * 2, MTS * 2, MTS * 4),
         output_image, (MTS * 10, MTS * 0))
    blit(
        image, (MTS * 8, MTS * 4, MTS * 10, MTS *
                6), output_image, (MTS * 12, MTS * 0)
    )
    blit(image, (MTS * 0, MTS * 0, MTS * 2, MTS * 2),
         output_image, (MTS * 14, MTS * 0))
    blit(
        image, (MTS * 8, MTS * 2, MTS * 10, MTS *
                4), output_image, (MTS * 16, MTS * 0)
    )
    blit(image, (MTS * 6, MTS * 4, MTS * 8, MTS * 6),
         output_image, (MTS * 18, MTS * 0))
    blit(image, (MTS * 4, MTS * 2, MTS * 6, MTS * 4),
         output_image, (MTS * 20, MTS * 0))
    blit(image, (MTS * 4, MTS * 0, MTS * 6, MTS * 2),
         output_image, (MTS * 22, MTS * 0))
    blit(image, (MTS * 2, MTS * 4, MTS * 4, MTS * 6),
         output_image, (MTS * 24, MTS * 0))
    blit(image, (MTS * 0, MTS * 4, MTS * 2, MTS * 6),
         output_image, (MTS * 26, MTS * 0))
    blit(image, (MTS * 4, MTS * 4, MTS * 6, MTS * 6),
         output_image, (MTS * 28, MTS * 0))
    # blit(image, (MTS * 2, MTS * 2, MTS * 4, MTS * 4), output_image, (MTS * 30, MTS * 0))

    return output_image


def unpack_image_a4(input_image, MTS=8) -> Image.Image:
    imw, imh = input_image.size
    INW, INH = (MTS * 4, MTS * 10)
    OUTW, OUTH = (MTS * 10, MTS * 12)
    rows, rwrem = divmod(imh, INH)
    cols, clrem = divmod(imw, INW)
    assert rwrem == 0, f"Invalid Height: ({imh})"
    assert clrem == 0, f"Invalid Width: ({imw})"
    outimw, outimh = cols * OUTW, rows * OUTH
    output_image = Image.new("RGBA", (outimw, outimh))

    for ii in range(rows):
        for jj in range(cols):
            terrain_subimage = input_image.crop(
                (jj * INW, ii * INH, (jj + 1) * INW, ((ii * INH) + MTS * 6))
            )
            converted = unpack_subimage_terraincell(terrain_subimage, MTS)
            output_image.paste(converted, (jj * OUTW, ii * OUTH))

            lateral_subimage = input_image.crop(
                (jj * INW, ((ii * INH) + (MTS * 6)),
                 (jj + 1) * INW, ((ii * INH) + (MTS * 10)))
            )
            converted = unpack_subimage_9cell(lateral_subimage, MTS)
            output_image.paste(converted, (jj * OUTW, ((ii * OUTH) + (MTS * 6))))
    return output_image


def unpack_image_a2(input_image, MTS=8):
    imw, imh = input_image.size
    INW, INH = (MTS * 4, MTS * 6)
    OUTW, OUTH = (MTS * 10, MTS * 6)

    rows, rwrem = divmod(imh, INH)
    cols, clrem = divmod(imw, INW)

    assert rwrem == 0, f"Invalid Height: ({imh})"
    assert clrem == 0, f"Invalid Width: ({imw})"

    outimw, outimh = cols * OUTW, rows * OUTH
    output_image = Image.new("RGBA", (outimw, outimh))

    for ii in range(rows):
        for jj in range(cols):
            subimage = input_image.crop(
                (jj * INW, ii * INH, (jj + 1) * INW, (ii + 1) * INH)
            )
            converted = unpack_subimage_terraincell(subimage, MTS)
            output_image.paste(converted, (jj * OUTW, ii * OUTH))

    return output_image


def blit(src, src_bounds, dst, dst_offs):
    dst.paste(src.crop((src_bounds)), dst_offs)


def pack_subimage(image, MTS):
    OUTW, OUTH = (MTS * 4, MTS * 6)
    output_image = Image.new("RGBA", (OUTW, OUTH))

    blit(image, (MTS * 0, MTS * 0, MTS * 2, MTS * 2),
         output_image, (MTS * 0, MTS * 2))
    blit(image, (MTS * 4, MTS * 4, MTS * 6, MTS * 6),
         output_image, (MTS * 2, MTS * 4))
    blit(image, (MTS * 0, MTS * 4, MTS * 2, MTS * 6),
         output_image, (MTS * 0, MTS * 4))
    blit(image, (MTS * 4, MTS * 0, MTS * 6, MTS * 2),
         output_image, (MTS * 2, MTS * 2))

    blit(image, (MTS * 6, MTS * 0, MTS * 7, MTS * 1),
         output_image, (MTS * 2, MTS * 0))
    blit(image, (MTS * 6, MTS * 3, MTS * 7, MTS * 4),
         output_image, (MTS * 2, MTS * 1))
    blit(image, (MTS * 9, MTS * 0, MTS * 10, MTS * 1),
         output_image, (MTS * 3, MTS * 0))
    blit(image, (MTS * 9, MTS * 3, MTS * 10, MTS * 4),
         output_image, (MTS * 3, MTS * 1))

    blit(image, (MTS * 0, MTS * 0, MTS * 1, MTS * 1),
         output_image, (MTS * 0, MTS * 0))
    blit(image, (MTS * 0, MTS * 5, MTS * 1, MTS * 6),
         output_image, (MTS * 0, MTS * 1))
    blit(image, (MTS * 5, MTS * 0, MTS * 6, MTS * 1),
         output_image, (MTS * 1, MTS * 0))
    blit(image, (MTS * 5, MTS * 5, MTS * 6, MTS * 6),
         output_image, (MTS * 1, MTS * 1))

    # check_cos = output_image.crop(((MTS*0, MTS*0, MTS*2, MTS*2)))

    return output_image


def unpack_subimage_9cell(image, MTS):
    # For GM, we fill the extra "inner" border with center tiles
    OUTW, OUTH = (MTS * 10, MTS * 6)
    output_image = Image.new("RGBA", (OUTW, OUTH))

    # UL
    blit(image, (MTS * 0, MTS * 0, MTS * 2, MTS * 2),
         output_image, (MTS * 0, MTS * 0))
    # UR
    blit(image, (MTS * 2, MTS * 0, MTS * 4, MTS * 2),
         output_image, (MTS * 4, MTS * 0))
    # DL
    blit(image, (MTS * 0, MTS * 2, MTS * 2, MTS * 4),
         output_image, (MTS * 0, MTS * 4))
    # DR
    blit(image, (MTS * 2, MTS * 2, MTS * 4, MTS * 4),
         output_image, (MTS * 4, MTS * 4))
    # LEFT
    blit(image, (MTS * 0, MTS * 1, MTS * 2, MTS * 2),
         output_image, (MTS * 0, MTS * 3))
    blit(image, (MTS * 0, MTS * 2, MTS * 2, MTS * 3),
         output_image, (MTS * 0, MTS * 2))
    # RIGHT
    blit(image, (MTS * 2, MTS * 1, MTS * 4, MTS * 2),
         output_image, (MTS * 4, MTS * 3))
    blit(image, (MTS * 2, MTS * 2, MTS * 4, MTS * 3),
         output_image, (MTS * 4, MTS * 2))
    # TOP
    blit(image, (MTS * 1, MTS * 0, MTS * 2, MTS * 2),
         output_image, (MTS * 3, MTS * 0))
    blit(image, (MTS * 2, MTS * 0, MTS * 3, MTS * 2),
         output_image, (MTS * 2, MTS * 0))
    # BOTTOM
    blit(image, (MTS * 1, MTS * 2, MTS * 2, MTS * 4),
         output_image, (MTS * 3, MTS * 4))
    blit(image, (MTS * 2, MTS * 2, MTS * 3, MTS * 4),
         output_image, (MTS * 2, MTS * 4))
    # CENTER
    blit(image, (MTS * 1, MTS * 1, MTS * 2, MTS * 2),
         output_image, (MTS * 3, MTS * 3))
    blit(image, (MTS * 2, MTS * 2, MTS * 3, MTS * 3),
         output_image, (MTS * 2, MTS * 2))
    blit(image, (MTS * 1, MTS * 2, MTS * 2, MTS * 3),
         output_image, (MTS * 3, MTS * 2))
    blit(image, (MTS * 2, MTS * 1, MTS * 3, MTS * 2),
         output_image, (MTS * 2, MTS * 3))

    # for GM (and even tiled), rewrite the center tiles to the blank area.
    for ii in range(6, 10, 2):
        for jj in range(0, 6, 2):
            blit(
                output_image, (MTS * 2, MTS * 2, MTS * 4, MTS * 4),
                output_image, (MTS * ii, MTS * jj)
            )

    return output_image


def unpack_subimage_terraincell(image, MTS):
    OUTW, OUTH = (MTS * 10, MTS * 6)  # 5 x 3
    output_image = Image.new("RGBA", (OUTW, OUTH))

    blit(image, (MTS * 0, MTS * 2, MTS * 4, MTS * 6),
         output_image, (MTS * 0, MTS * 0))
    blit(
        output_image,
        (MTS * 0, MTS * 2, MTS * 4, MTS * 4),
        output_image,
        (MTS * 0, MTS * 4),
    )
    blit(
        output_image,
        (MTS * 2, MTS * 0, MTS * 4, MTS * 6),
        output_image,
        (MTS * 4, MTS * 0),
    )
    blit(
        output_image,
        (MTS * 0, MTS * 1, MTS * 6, MTS * 2),
        output_image,
        (MTS * 0, MTS * 3),
    )
    blit(
        output_image,
        (MTS * 0, MTS * 4, MTS * 6, MTS * 5),
        output_image,
        (MTS * 0, MTS * 2),
    )
    blit(
        output_image,
        (MTS * 4, MTS * 0, MTS * 5, MTS * 6),
        output_image,
        (MTS * 2, MTS * 0),
    )
    blit(
        output_image,
        (MTS * 1, MTS * 0, MTS * 2, MTS * 6),
        output_image,
        (MTS * 3, MTS * 0),
    )

    blit(
        output_image,
        (MTS * 2, MTS * 2, MTS * 4, MTS * 4),
        output_image,
        (MTS * 6, MTS * 0),
    )
    blit(
        output_image,
        (MTS * 2, MTS * 2, MTS * 4, MTS * 4),
        output_image,
        (MTS * 8, MTS * 0),
    )
    blit(
        output_image,
        (MTS * 2, MTS * 2, MTS * 4, MTS * 4),
        output_image,
        (MTS * 6, MTS * 2),
    )
    blit(
        output_image,
        (MTS * 2, MTS * 2, MTS * 4, MTS * 4),
        output_image,
        (MTS * 8, MTS * 2),
    )
    blit(
        output_image,
        (MTS * 2, MTS * 2, MTS * 4, MTS * 4),
        output_image,
        (MTS * 6, MTS * 4),
    )
    blit(
        output_image,
        (MTS * 2, MTS * 2, MTS * 4, MTS * 4),
        output_image,
        (MTS * 8, MTS * 4),
    )

    blit(
        image, (MTS * 2, MTS * 0, MTS * 3, MTS *
                1), output_image, (MTS * 6, MTS * 0)
    )  # UL
    blit(
        image, (MTS * 3, MTS * 1, MTS * 4, MTS *
                2), output_image, (MTS * 9, MTS * 3)
    )  # BR
    blit(
        image, (MTS * 3, MTS * 0, MTS * 4, MTS *
                1), output_image, (MTS * 9, MTS * 0)
    )  # UR
    blit(
        image, (MTS * 2, MTS * 1, MTS * 3, MTS *
                2), output_image, (MTS * 6, MTS * 3)
    )  # BL

    blit(
        image, (MTS * 2, MTS * 0, MTS * 3, MTS *
                1), output_image, (MTS * 6, MTS * 4)
    )  # UL
    blit(
        image, (MTS * 3, MTS * 1, MTS * 4, MTS *
                2), output_image, (MTS * 7, MTS * 5)
    )  # BR
    blit(
        image, (MTS * 3, MTS * 0, MTS * 4, MTS *
                1), output_image, (MTS * 9, MTS * 4)
    )  # UR
    blit(
        image, (MTS * 2, MTS * 1, MTS * 3, MTS *
                2), output_image, (MTS * 8, MTS * 5)
    )  # BL

    return output_image


def main() -> int:
    ap = ArgumentParser()
    sp = ap.add_subparsers()

    cmd = sp.add_parser("scale")
    cmd.add_argument("input_filename", nargs="+")
    cmd.add_argument(
        "--factor",
        type=float,
        required=True,
        help="the factor to scale by (e.g., 2 for 2x)",
    )
    cmd.set_defaults(cmd=cmd_scale)

    cmd = sp.add_parser("unpack")
    cmd.add_argument("format", choices=["a2", "a4"])
    cmd.add_argument("input_filename", nargs="+")
    cmd.add_argument(
        "--gms",
        choices=[16],
        type=int,
        help="export to GMS format (only 16 supported for now)",
    )
    cmd.add_argument(
        "--mts",
        "--minitile-size",
        type=int,
        default=8,
        help="The size of the minitile.  8 for 16px tiles.",
    )
    cmd.set_defaults(cmd=cmd_unpack)

    cmd = sp.add_parser("pack")
    cmd.add_argument("format", choices=["a2"])
    cmd.add_argument("input_filename", nargs="+")
    cmd.add_argument(
        "--mts",
        "--minitile-size",
        type=int,
        default=8,
        help="The size of the minitile.  8 for 16px tiles.",
    )
    cmd.set_defaults(cmd=cmd_pack)

    ns = ap.parse_args()
    if "cmd" not in ns:
        ap.print_help()
        return 2
    return ns.cmd(ns)


def cmd_scale(ns: Any) -> int:
    for input_filename in ns.input_filename:
        upscale_file(input_filename, ns.factor)
    return 0


def cmd_unpack(ns: Any) -> int:
    unpack_func = unpack_image_a2 if ns.format == "a2" else unpack_image_a4
    suffix = "_expanded-gms16" if ns.gms == 16 else "_expanded"
    for input_filename in ns.input_filename:
        p1, p2 = os.path.splitext(input_filename)
        output_filename = p1 + suffix + p2

        input_image = Image.open(input_filename)
        output_image = unpack_func(input_image, MTS=ns.mts)
        if ns.gms is not None:
            output_image = flatten_tiled_file_to_gm16(
                output_image, MTS=ns.mts, zigzag=(ns.format == "a4")
            )
        output_image.save(output_filename)

    return 0


def cmd_pack(ns: Any) -> int:
    for input_filename in ns.input_filename:
        pack_file(input_filename, MTS=ns.mts)
    return 0


if __name__ == "__main__":
    sys.exit(main())
