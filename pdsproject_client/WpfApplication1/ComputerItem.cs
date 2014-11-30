using System;

namespace Views.ViewsPOCO
{
    public class ComputerItem
    {
        public String Name { get; set; }
        public String ComputerStateImage { get; set; }                
        public int ComputerNum { get; set; }
        public int ComputerID { get; set; }
        public bool IsCheckboxEnabled { get; set; }
        public bool IsCheckboxChecked { get; set; }
    }
}
