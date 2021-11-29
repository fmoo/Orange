from enum import IntEnum, IntFlag
from io import BytesIO
import struct
import warnings

from typing import List, Optional

## https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md


class ChunkType(IntEnum):
    # Ignore this chunk if you find the new palette chunk (0x2019) Aseprite v1.1 saves both chunks 0x0004 and 0x2019 just for backward compatibility.
    OldPaletteChunk = 0x0004
    # Ignore this chunk if you find the new palette chunk (0x2019)
    OldPaletteChunk2 = 0x0011
    # In the first frame should be a set of layer chunks to determine the entire layers layout:
    LayerChunk = 0x2004
    # This chunk determine where to put a cel in the specified layer/frame.
    CelChunk = 0x2005
    # Adds extra information to the latest read cel.
    CelExtraChunk = 0x2006
    # Color profile for RGB or grayscale values.
    ColorProfileChunk = 0x2007
    # DEPRECATED
    MaskChunk_DEPRECATED = 0x2016
    # Never used.
    PathChunk_UNUSED = 0x2017
    # No Information
    TagsChunk = 0x2018
    # No Information
    PaletteChunk = 0x2019
    # Insert this user data in the last read chunk. E.g. If we've read a layer, this user data belongs to that layer, if we've read a cel, it belongs to that cel, etc.
    UserDataChunk = 0x2020
    # No Information
    SliceChunk = 0x2022


class AsepriteIO(BytesIO):
    PIXELSIZE = 4

    def set_pixelsize(self, pixelsize):
        self.PIXELSIZE = pixelsize
        return self

    def readbyte(self):
        dat = self.read(1)
        (val,) = struct.unpack("<B", dat)
        return val

    def readword(self):
        dat = self.read(2)
        (val,) = struct.unpack("<H", dat)
        return val

    def readshort(self):
        dat = self.read(2)
        (val,) = struct.unpack("<h", dat)
        return val

    def readdword(self):
        dat = self.read(4)
        (val,) = struct.unpack("<L", dat)
        return val

    def readlong(self):
        dat = self.read(4)
        (val,) = struct.unpack("<l", dat)
        return val

    def readfixed(self):
        dat = self.read(4)
        (num, den) = struct.unpack("<hh", dat)  # probably not correct
        return (num, den)

    def readbytearr(self, n):
        return self.read(n)

    def readstr(self):
        strlen = self.readword()
        strdata = self.readbytearr(strlen)
        return strdata.decode("utf-8")

    def readpixel(self):
        return tuple(self.readbytearr(self.PIXELSIZE))

    def readheader(self):
        data = self
        filesize = data.readdword()
        magic = data.readword()
        assert magic == 0xA5E0
        frames = data.readword()
        width = data.readword()
        height = data.readword()
        bpp = data.readword()
        data.PIXELSIZE = bpp >> 3
        flags = data.readdword()
        _speed_DEPRECATED = data.readword()
        _ = data.readdword()
        _ = data.readdword()
        transparentindex = data.readbyte()
        _ = data.readbytearr(3)
        numcolors = data.readword()
        pxwidth = data.readbyte()
        pxheight = data.readbyte()
        gridx = data.readshort()
        gridy = data.readshort()
        gridw = data.readword()
        gridh = data.readword()
        _ = data.readbytearr(84)
        return {
            "filesize": filesize,
            "size": (width, height),
            "flags": flags,
            "numframes": frames,
            "transparentindex": transparentindex,
            "numcolors": numcolors,
            "pxwidth": pxwidth,
            "phheight": pxheight,
            "grid": {"x": gridx, "y": gridy, "w": gridw, "h": gridh},
        }

    def readframeheader(self):
        """
        DWORD       Bytes in this frame
        WORD        Magic number (always 0xF1FA)
        WORD        Old field which specifies the number of "chunks"
                    in this frame. If this value is 0xFFFF, we might
                    have more chunks to read in this frame
                    (so we have to use the new field)
        WORD        Frame duration (in milliseconds)
        BYTE[2]     For future (set to zero)
        DWORD       New field which specifies the number of "chunks"
                    in this frame (if this is 0, use the old field)
        """
        numbytes = self.readdword()
        magic = self.readword()
        assert magic == 0xF1FA
        oldchunks = self.readword()
        frameduration = self.readword()
        _ = self.readbytearr(2)
        newchunks = self.readdword()
        numchunks = oldchunks
        if oldchunks == 0xFFFF and newchunks != 0:
            numchunks = newchunks
        return {
            "framebytes": numbytes,
            "frameduration": frameduration,
            "numchunks": numchunks,
        }

    def readchunk(self):
        """
        DWORD       Chunk size
        WORD        Chunk type
        BYTE[]      Chunk data
        """
        chunksize = self.readdword()
        chunktype = ChunkType(self.readword())
        chunkdata = self.readbytearr(chunksize - 6)
        return {
            "type": chunktype,
            "data": _ParseChunk(chunktype, chunkdata, self.PIXELSIZE),
        }


import warnings
import zlib


def _ParseChunk(chunkType: ChunkType, chunkBytes: bytes, pixelsize: int):
    data = AsepriteIO(chunkBytes).set_pixelsize(pixelsize)

    if chunkType == ChunkType.CelChunk:
        """
        WORD        Layer index (see NOTE.2)
        SHORT       X position
        SHORT       Y position
        BYTE        Opacity level
        WORD        Cel type
        BYTE[7]     For future (set to zero)
        + For cel type = 0 (Raw Cel)
        WORD      Width in pixels
        WORD      Height in pixels
        PIXEL[]   Raw pixel data: row by row from top to bottom,
                    for each scanline read pixels from left to right.
        + For cel type = 1 (Linked Cel)
        WORD      Frame position to link with
        + For cel type = 2 (Compressed Image)
        WORD      Width in pixels
        WORD      Height in pixels
        BYTE[]    "Raw Cel" data compressed with ZLIB method
        """
        result = {}
        result["layerIndex"] = data.readword()
        result["xPos"] = data.readshort()
        result["yPos"] = data.readshort()
        result["opacity"] = data.readbyte()
        celType = data.readword()
        result["type"] = celType

        _ = data.readbytearr(7)
        if celType == 2:  # Compressed Image
            # Decompress the
            datapos = data.tell()
            _celWidth = data.readword()
            _celHeight = data.readword()
            compressed = data.read()
            decompressed = zlib.decompress(compressed)
            # Overwrite the compressed data
            data.seek(datapos + 4)
            data.write(decompressed)
            data.truncate()
            data.seek(datapos)
            result["compressed"] = True

        if celType == 1:  # Linked Cell
            """WORD      Frame position to link with"""
            framePosition = data.readword()  # wtf do I do with this.
            result["framePosition"] = framePosition

        elif celType in [0, 2]:  # Raw Image (or decompressed compressed cel)
            celWidth = data.readword()
            celHeight = data.readword()
            result["width"] = celWidth
            result["height"] = celHeight
            result["pixels"] = []
            for _ii in range(celHeight):
                row: List = []
                result["pixels"].append(row)
                for _jj in range(celWidth):
                    pixel = data.readpixel()
                    row.append(pixel)

        return result

    elif chunkType == ChunkType.LayerChunk:
        result = {}
        result["flags"] = Layer.Flag(data.readword())
        result["type"] = Layer.Type(data.readword())
        result["childLevel"] = data.readword()
        _defaultLayerWidth = data.readword()
        _defaultLayerHeight = data.readword()
        result["blendMode"] = Layer.BlendMode(data.readword())
        # only valid if file header flags field has bit 1 set?
        result["opacity"] = data.readbyte()
        _ = data.readbytearr(3)
        result["name"] = data.readstr()
        return result

    elif chunkType in [ChunkType.OldPaletteChunk, ChunkType.OldPaletteChunk2]:
        result = {"colors": []}
        numPackets = data.readword()
        result["numpackets"] = numPackets
        for _ii in range(numPackets):
            entryOffset = data.readbyte()
            while entryOffset > 0:
                result["colors"].append((0, 0, 0))
                entryOffset -= 1
            numColors = data.readbyte()
            numColors = 256 if numColors == 0 else numColors
            for _jj in range(numColors):
                r = data.readbyte()
                g = data.readbyte()
                b = data.readbyte()
                result["colors"].append((r, g, b))
        return result

    elif chunkType == ChunkType.ColorProfileChunk:
        result = {}
        result["type"] = ColorProfile.Type(data.readword())
        result["flags"] = ColorProfile.Flags(data.readword())
        result["gamma"] = data.readfixed()
        _ = data.readbytearr(8)
        if result["type"] == ColorProfile.Type.Embedded:
            icclen = data.readdword()
            result["icc_profile"] = data.readbytearr(icclen)
        return result

    elif chunkType == ChunkType.PaletteChunk:
        # DWORD       New palette size (total number of entries)
        # DWORD       First color index to change
        # DWORD       Last color index to change
        # BYTE[8]     For future (set to zero)
        # + For each palette entry in [from,to] range (to-from+1 entries)
        # WORD      Entry flags:
        #             1 = Has name
        # BYTE      Red (0-255)
        # BYTE      Green (0-255)
        # BYTE      Blue (0-255)
        # BYTE      Alpha (0-255)
        # + If has name bit in entry flags
        #     STRING  Color name
        result = {"colors": {}}
        numEntries = data.readdword()
        indexfirst = data.readdword()
        indexlast = data.readdword()
        assert numEntries == indexlast - indexfirst + 1
        _ = data.read(8)
        for nn in range(indexfirst, indexlast + 1):
            flags = Palette.Flags(data.readword())
            r = data.readbyte()
            g = data.readbyte()
            b = data.readbyte()
            a = data.readbyte()
            color = {"value": (r, g, b, a)}
            if Palette.Flags.HasColorName in flags:
                color["name"] = data.readstr()
            result["colors"][nn] = color

        return result

    else:
        warnings.warn(f"Chunk type 0x{'%x'%chunkType} not yet supported")
        return None


class Palette:
    class Flags(IntFlag):
        HasColorName = 1


class ColorProfile:
    class Type(IntEnum):
        NoProfile = 0
        SRGB = 1
        Embedded = 2

    class Flags(IntFlag):
        UseSpecialFixedGamma = 1


class Layer:
    class Type(IntEnum):
        ImageLayer = 0
        LayerGroup = 1

    class Flag(IntFlag):
        Visible = 1
        Editable = 2
        LockMovement = 4
        Background = 8
        PreferLinkedCels = 16
        LayerGroupCollapsed = 32
        ReferenceLayer = 64

    class BlendMode(IntEnum):
        Normal = 0
        Multiply = 1
        Screen = 2
        Overlay = 3
        Darken = 4
        Lighten = 5
        ColorDodge = 6
        ColorBurn = 7
        HardLight = 8
        SoftLight = 9
        Difference = 10
        Exclusion = 11
        Hue = 12
        Saturation = 13
        Color = 14
        Luminosity = 15
        Addition = 16
        Subtract = 17
        Divide = 18


import pprint
from pathlib import Path


def dump_file(input_filename) -> None:
    inp = Path(input_filename)
    data = AsepriteIO(inp.read_bytes())
    print(f"## {inp} ##")
    print("\nHeader:")
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
            pprint.pprint(chunk)
            # pprint.pprint(chunk)
        # _framedata = data.readbytearr(numbytes)


from PIL import Image
import PIL


def extract_images(input_filename) -> None:
    inp = Path(input_filename)
    data = AsepriteIO(inp.read_bytes())
    header = data.readheader()

    layers = []
    for framenumber in range(header["numframes"]):
        frameheader = data.readframeheader()
        for _jj in range(frameheader["numchunks"]):
            chunk = data.readchunk()

            if chunk["type"] == ChunkType.LayerChunk:
                layers.append(chunk["data"])
            if chunk["type"] == ChunkType.CelChunk:
                cel = chunk["data"]
                image = Image.new("RGBA", header["size"])
                for yy, row in enumerate(cel["pixels"]):
                    for xx, vv in enumerate(row):
                        # print(f"image.putpixel(({xx}, {yy}), {vv})")
                        image.putpixel((xx + cel["xPos"], yy + cel["yPos"]), vv)
                outpath = inp.with_name(
                    f"{inp.stem}-{layers[cel['layerIndex']]['name']}-{framenumber}.png"
                )
                print(f"Writing {outpath}...")
                image.save(outpath)

    pprint.pprint(layers)
    # pprint.pprint(chunk)
    # _framedata = data.readbytearr(numbytes)


def extract_frame(input_filename: str, frame_index: int) -> Optional[Image.Image]:
    inp = Path(input_filename)
    data = AsepriteIO(inp.read_bytes())
    header = data.readheader()
    image = Image.new("RGBA", header["size"])

    layers = []
    lastlayerdepth = {}
    found_frame = False

    for framenumber in range(header["numframes"]):

        frameheader = data.readframeheader()
        for _jj in range(frameheader["numchunks"]):
            chunk = data.readchunk()

            if chunk["type"] == ChunkType.LayerChunk:
                layer = chunk["data"]
                lastlayerdepth[layer["childLevel"]] = layer

                curparentLevel = layer["childLevel"] - 1
                while curparentLevel >= 0:
                    parent = lastlayerdepth[curparentLevel]
                    if Layer.Flag.Visible not in parent["flags"]:
                        layer["flags"] &= ~Layer.Flag.Visible

                    curparentLevel -= 1

                layers.append(layer)
            if chunk["type"] == ChunkType.CelChunk:
                cel = chunk["data"]
                if cel["opacity"] == 0:
                    continue
                if layers[cel["layerIndex"]]["opacity"] == 0:
                    continue
                # TODO: HANDLE PARENT LAYER VISIBILITY
                if Layer.Flag.Visible not in layers[cel["layerIndex"]]["flags"]:
                    continue


                if framenumber + 1 != frame_index:
                    continue
                found_frame = True

                for yy, row in enumerate(cel["pixels"]):
                    for xx, vv in enumerate(row):
                        if vv[3] == 0:
                            continue
                        # print(f"image.putpixel(({xx}, {yy}), {vv})")
                        image.putpixel((xx + cel["xPos"], yy + cel["yPos"]), vv)

    if not found_frame:
        warnings.warn(f"Frame not found: {frame_index}")
        return None

    return image
