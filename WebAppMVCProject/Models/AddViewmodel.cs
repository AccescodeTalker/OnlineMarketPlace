using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppMVCProject.Models
{
    public class AddViewmodel
    {
        public int pId { get; set; }
        public string pName { get; set; }
        public string pimg { get; set; }
        public string pdesc { get; set; }
        public Nullable<int> pprice { get; set; }
        public Nullable<int> pFKuser { get; set; }
        public Nullable<int> pFKcat { get; set; }
        public int cId { get; set; }
        public string cName { get; set; }
        public string uName { get; set; }
        public string ucon { get; set; }
    }
}