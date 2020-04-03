AS_FLAGS=--gdwarf-2 -o 
LD_FLAGS=--static
SOURCES=remember_all.s
OBJECTS=$(SOURCES:.s=.o)
EXECUTABLE=remember_all

all: $(SOURCES)	$(EXECUTABLE)

$(EXECUTABLE): $(OBJECTS)
	ld $(LD_FLAGS) $(OBJECTS) -o $@

.s.o:
	as $(AS_FLAGS) $@ -c $<

clean:
	rm -f *.o
