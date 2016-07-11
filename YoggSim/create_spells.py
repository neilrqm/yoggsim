#!/usr/bin/python

"""Create a Spells.cs file for the spells listed in spells.txt.  Preserves
existing code in Spells.cs."""

import re
import shutil
import os

class Spell(object):
    Name = ""
    Text = ""
    Code = ""

spellFile = open("spells.txt", "r")

spells = {}
spellsOrder = []

lines = spellFile.readlines()

spellFile.close()

for i in range(1, len(lines), 3):
    s = Spell()
    s.Name = lines[i].strip()
    s.Text = lines[i+1].strip()
    spells[s.Name] = s
    spellsOrder.append(s.Name)

spellCs = open("Spells.cs", "r")

currentSpell = None
for l in spellCs.readlines():
    m = re.search("public static void (?P<name>\w+)\(Board b\)", l)
    if m:
        currentSpell = spells[m.group("name")]
        continue
    if currentSpell is not None:
        if l.rstrip() == "        {":
            continue
        if l.rstrip() == "        }":
            currentSpell = None
            continue;
        currentSpell.Code += l

spellCs.close()
shutil.copyfile("Spells.cs", "Spells.cs.bak")
os.remove("Spells.cs")

spellCs = open("Spells.cs", "w")
spellCs.write("""using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    abstract class Spells
    {
""")
for s in spellsOrder:
    spellCs.write("""        /// <summary>
        /// %s
        /// </summary>
        public static void %s(Board b)
        {
%s
        }
        
"""%(spells[s].Text, spells[s].Name, spells[s].Code.rstrip()))
    
spellCs.write("""    }
}
""")
spellCs.close()
