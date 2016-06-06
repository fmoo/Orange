BUILDDIR = ../Tiled2Unity
SOURCES = $(wildcard *.tmx)
OBJECTS = $(patsubst %.tmx, $(BUILDDIR)/Imported/%.tiled2unity.xml, $(SOURCES))

%.tiled2unity.xml:
	Tiled2UnityLite.exe --verbose -s 0.01 $(*F).tmx $(BUILDDIR)

all: $(OBJECTS)
