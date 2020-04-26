"""Opens an RPGMaker sprite subsheet and writes it to an .aseprite file"""

from argparse import ArgumentParser
from pathlib import Path
import PIL
from PIL import Image
from io import BytesIO
from aseprite import AsepriteIO, ChunkType, ParseChunk
import struct
import pprint


## https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md
def aseprite2char(input_filename):
    inp = Path(input_filename)
    data = AsepriteIO(inp.read_bytes())
    header = data.readheader()
    pprint.pprint(header)
    for ii in range(header["numframes"]):
        frameheader = data.readframeheader()
        print(f"\nFrame #{ii+1}:")
        pprint.pprint(frameheader)
        # numbytes = frameheader["framebytes"]

        for jj in range(frameheader["numchunks"]):
            chunk = data.readchunk()
            chunktype = ChunkType(chunk["type"]).name
            print(f"Chunk #{ii+1}.{jj+1} ({chunktype})")

            pprint.pprint(ParseChunk(chunk, data.PIXELSIZE))
            # pprint.pprint(chunk)
        # _framedata = data.readbytearr(numbytes)


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
        aseprite2char(fn)


if __name__ == "__main__":
    main()
