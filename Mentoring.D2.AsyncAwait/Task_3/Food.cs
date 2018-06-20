using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_3
{
    public class Food
    {
        [Browsable(false)]
        public int Id { get; set; }
        public String Name { get; set; }
        public int Price { get; set; }
    }
}
