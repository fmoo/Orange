from io import BytesIO
import struct


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
        (val,) = struct.unpack("<f", dat)  # probably not correct
        return val

    def readbytearr(self, n):
        return self.read(n)

    def readstr(self):
        strlen = self.readword()
        strdata = self.readbytearr(strlen)
        return strdata.decode("utf-8")

    def readpixel(self):
        return self.readbytearr(self.PIXELSIZE)

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
        chunktype = self.readword()
        chunkdata = self.readbytearr(chunksize - 6)
        return {
            "type": chunktype,
            "data": chunkdata,
        }


import warnings
import zlib


def ParseChunk(chunk: dict, pixelsize: int):
    if chunk["type"] not in {item.value for item in ChunkType}:
        warnings.warn(f"Unknown chunk type: 0x{'%x'%chunk['type']}")
        return None
    chunkType = ChunkType(chunk["type"])
    data = AsepriteIO(chunk["data"]).set_pixelsize(pixelsize)

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
                row = []
                result["pixels"].append(row)
                for _jj in range(celWidth):
                    pixel = data.readpixel()
                    row.append(pixel)

        return result
    else:
        warnings.warn(f"Chunk type {chunkType} not yet supported")
        return None


from enum import Enum


class ChunkType(Enum):
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
