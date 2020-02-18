from PIL import Image, ImageDraw, ImageFont
from argparse import ArgumentParser
from pathlib import Path

ap = ArgumentParser()
ap.add_argument('--tilesize', type=int, default=16)
ap.add_argument('--rows', type=int, default=10)
num_tiles = 10

ns = ap.parse_args()
size = ns.tilesize * ns.rows

p = Path('.') / 'Fonts' / 'Romulus_by_pix3m-d6aokem.ttf'

fnt = ImageFont.truetype(str(p), 16)
img = Image.new('RGBA', (size, size), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
nn = 0

for ii in range(ns.rows):
    for jj in range(ns.rows):
        msg = str(nn)
        (msgw, msgh) = fnt.getsize(msg)
        xoffs = (ns.tilesize - msgw) / 2
        yoffs = -2 + (ns.tilesize - msgh) / 2       
        d.text((jj * ns.tilesize + xoffs + 1, ii * ns.tilesize + yoffs + 1), msg, align="center", font=fnt, fill=(0, 0, 0, 255))
        d.text((jj * ns.tilesize + xoffs, ii * ns.tilesize + yoffs), msg, align="center", font=fnt, fill=(255, 255, 255, 255))
        nn += 1
img.save('Numbers.png')

with open('Numbers.tsx', mode='w') as f:
    f.write(f'<?xml version="1.0" encoding="UTF-8"?>\n')
    f.write(f'<tileset version="1.2" tiledversion="1.3.2" name="Numbers" tilewidth="{ns.tilesize}" tileheight="{ns.tilesize}" tilecount="100" columns="{ns.rows}">\n')
    f.write(f'  <image source="Numbers.png" width="{size}" height="{size}"/>\n')
    for nn in range(0, ns.rows ** 2):
        f.write(f'  <tile id="{nn}">\n')
        f.write(f'    <properties>\n')
        f.write(f'      <property name="Value" type="int" value="{nn}"/>\n')
        f.write(f'    </properties>\n')
        f.write(f'  </tile>\n')
    f.write(f'</tileset>')

