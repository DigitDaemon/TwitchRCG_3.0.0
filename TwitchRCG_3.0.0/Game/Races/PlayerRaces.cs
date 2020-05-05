using Game.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Races
{
    class Human : Race
    {
        public Human() : base(69,13,13,13,7)
        {

        }
    }

    class Elf : Race
    {
        public Elf() : base(60, 19, 7,13,12)
        {

        }
    }

    class HalfElf : Race
    {
        public HalfElf() : base(63,15,11,13,10)
        {

        }
        
    }

    class Dwarf : Race
    {
        public Dwarf() : base(80, 5,20,15,4)
        {

        }
    }

    class Tiefling : Race
    {
        public Tiefling() : base(50,18,10,6,15)
        {

        }
        
    }

    class Orc : Race
    {
        public Orc() : base(85,6,22,6,9)
        {

        }
        
    }

    class Gnome : Race
    {
        public Gnome() : base(50,16,8,16,10)
        {

        }
        
    }

    class Halfling : Race
    {
        public Halfling() : base(45,13,8,19,12)
        {

        }
        
    }

    class Drow : Race
    {
        public Drow() : base(60,15,10,11,13)
        {

        }
       
    }
}
