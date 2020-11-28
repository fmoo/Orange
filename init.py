# /usr/bin/env python3

import collections
import json
from pathlib import Path
from argparse import ArgumentParser
import os

ORANGE_ROOT = Path(globals().get("__file__", "./_")).absolute().parent
cwd = Path(os.getcwd())

ap = ArgumentParser()
ap.add_argument('location', nargs='?', default=os.getcwd())
ns = ap.parse_args()


def get_repo_root(curdir):
    while curdir is not None:
        maybe_assets = curdir / 'Assets'
        if maybe_assets.exists() and maybe_assets.is_dir():
            return curdir
        curdir = curdir.parent


REPO_ROOT = get_repo_root(Path(ns.location))

# TODO Mklink
WANT_PACKAGES = {
    "com.neuecc.unirx": "https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts",
    "com.dbrizov.naughtyattributes": "https://github.com/dbrizov/NaughtyAttributes.git#upm",
    "com.unity.2d.pixel-perfect": "2.0.4",
    "com.unity.ide.vscode": "1.2.3",
}


manifest_path = REPO_ROOT / 'Packages' / 'manifest.json'
manifest = json.loads(manifest_path.read_text(),
                      object_pairs_hook=collections.OrderedDict)
changed = False
for k, v in WANT_PACKAGES.items():
    if k not in manifest['dependencies']:
        print(f'{k}: Added!')
        manifest['dependencies'][k] = v
        changed = True
        continue
    if k == 'com.unity.ide.vscode' and manifest['dependencies'][k] < v:
        print(f'{k}: Updated to {v}')
        manifest['dependencies'][k] = v
        changed = True
        continue
    print(f'{k}: Manifest OK')

if changed:
    print(f'Saving changes to "{str(manifest_path)}"...')
    manifest_path.write_text(json.dumps(manifest))

COPY = [
    'omnisharp.json'
]

for p in COPY:
    contents = (ORANGE_ROOT / p).read_text()
    dest = REPO_ROOT / p
    if not dest.exists() or dest.read_text() != contents:
        print(f'{p}: Updated')
        dest.write_text(contents)
    else:
        print(f'{p}: OK')
        