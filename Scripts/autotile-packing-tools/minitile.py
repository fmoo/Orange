import os.path
import PIL
from PIL import Image


def upscale_file(input_filename, factor):
    p1, p2 = os.path.splitext(input_filename)
    output_filename = f'{p1}_{factor}x{p2}'
    input_image = Image.open(input_filename)
    imw, imh = input_image.size
    output_image = input_image.resize((imw * factor, imh * factor), Image.NEAREST)
    output_image.save(output_filename)


def pack_file(input_filename, MTS=8):
    p1, p2 = os.path.splitext(input_filename)
    output_filename = p1 + '_packed' + p2
    input_image = Image.open(input_filename)
    imw, imh = input_image.size
    INW, INH = (MTS*10, MTS*6)
    OUTW, OUTH = (MTS*4, MTS*6)

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
            converted = pack_subimage(subimage, MTS)
            output_image.paste(converted, (jj * OUTW, ii * OUTH))

    output_image.save(output_filename)


def unpack_file(input_filename, MTS=8):
    p1, p2 = os.path.splitext(input_filename)
    output_filename = p1 + '_expanded' + p2
    input_image = Image.open(input_filename)
    imw, imh = input_image.size
    INW, INH = (MTS*4, MTS*6)
    OUTW, OUTH = (MTS*10, MTS*6)

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
            converted = unpack_subimage(subimage, MTS)
            output_image.paste(converted, (jj * OUTW, ii * OUTH))

    output_image.save(output_filename)


def blit(src, src_bounds, dst, dst_offs):
    dst.paste(src.crop((src_bounds)), dst_offs)


def pack_subimage(image, MTS):
    OUTW, OUTH = (MTS*4, MTS*6)
    output_image = Image.new("RGBA", (OUTW, OUTH))

    blit(image, (MTS*0, MTS*0, MTS*2, MTS*2), output_image, (MTS*0, MTS*2))
    blit(image, (MTS*4, MTS*4, MTS*6, MTS*6), output_image, (MTS*2, MTS*4))
    blit(image, (MTS*0, MTS*4, MTS*2, MTS*6), output_image, (MTS*0, MTS*4))
    blit(image, (MTS*4, MTS*0, MTS*6, MTS*2), output_image, (MTS*2, MTS*2))

    blit(image, (MTS*6, MTS*0, MTS*7, MTS*1), output_image, (MTS*2, MTS*0))
    blit(image, (MTS*6, MTS*3, MTS*7, MTS*4), output_image, (MTS*2, MTS*1))
    blit(image, (MTS*9, MTS*0, MTS*10, MTS*1), output_image, (MTS*3, MTS*0))
    blit(image, (MTS*9, MTS*3, MTS*10, MTS*4), output_image, (MTS*3, MTS*1))

    blit(image, (MTS*0, MTS*0, MTS*1, MTS*1), output_image, (MTS*0, MTS*0))
    blit(image, (MTS*0, MTS*5, MTS*1, MTS*6), output_image, (MTS*0, MTS*1))
    blit(image, (MTS*5, MTS*0, MTS*6, MTS*1), output_image, (MTS*1, MTS*0))
    blit(image, (MTS*5, MTS*5, MTS*6, MTS*6), output_image, (MTS*1, MTS*1))

    # check_cos = output_image.crop(((MTS*0, MTS*0, MTS*2, MTS*2)))

    return output_image


def unpack_subimage(image, MTS):
    OUTW, OUTH = (MTS * 10, MTS * 6)
    output_image = Image.new("RGBA", (OUTW, OUTH))

    blit(image, (MTS*0, MTS*2, MTS*4, MTS*6), output_image, (MTS*0, MTS*0))
    blit(output_image, (MTS*0, MTS*2, MTS*4, MTS*4), output_image, (MTS*0, MTS*4))
    blit(output_image, (MTS*2, MTS*0, MTS*4, MTS*6), output_image, (MTS*4, MTS*0))
    blit(output_image, (MTS*0, MTS*1, MTS*6, MTS*2), output_image, (MTS*0, MTS*3))
    blit(output_image, (MTS*0, MTS*4, MTS*6, MTS*5), output_image, (MTS*0, MTS*2))
    blit(output_image, (MTS*4, MTS*0, MTS*5, MTS*6), output_image, (MTS*2, MTS*0))
    blit(output_image, (MTS*1, MTS*0, MTS*2, MTS*6), output_image, (MTS*3, MTS*0))

    blit(output_image, (MTS*2, MTS*2, MTS*4, MTS*4), output_image, (MTS*6, MTS*0))
    blit(output_image, (MTS*2, MTS*2, MTS*4, MTS*4), output_image, (MTS*8, MTS*0))
    blit(output_image, (MTS*2, MTS*2, MTS*4, MTS*4), output_image, (MTS*6, MTS*2))
    blit(output_image, (MTS*2, MTS*2, MTS*4, MTS*4), output_image, (MTS*8, MTS*2))
    blit(output_image, (MTS*2, MTS*2, MTS*4, MTS*4), output_image, (MTS*6, MTS*4))
    blit(output_image, (MTS*2, MTS*2, MTS*4, MTS*4), output_image, (MTS*8, MTS*4))

    blit(image, (MTS*2, MTS*0, MTS*3, MTS*1),
         output_image, (MTS*6, MTS*0))  # UL
    blit(image, (MTS*3, MTS*1, MTS*4, MTS*2),
         output_image, (MTS*9, MTS*3))  # BR
    blit(image, (MTS*3, MTS*0, MTS*4, MTS*1),
         output_image, (MTS*9, MTS*0))   # UR
    blit(image, (MTS*2, MTS*1, MTS*3, MTS*2),
         output_image, (MTS*6, MTS*3))  # BL

    blit(image, (MTS*2, MTS*0, MTS*3, MTS*1),
         output_image, (MTS*6, MTS*4))  # UL
    blit(image, (MTS*3, MTS*1, MTS*4, MTS*2),
         output_image, (MTS*7, MTS*5))  # BR
    blit(image, (MTS*3, MTS*0, MTS*4, MTS*1),
         output_image, (MTS*9, MTS*4))   # UR
    blit(image, (MTS*2, MTS*1, MTS*3, MTS*2),
         output_image, (MTS*8, MTS*5))  # BL

    return output_image
