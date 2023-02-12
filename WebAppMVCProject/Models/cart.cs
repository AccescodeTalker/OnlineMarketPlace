using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppMVCProject.Models
{
    public class cart
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float price { get; set; }
        public int qty { get; set; }
        public float bill { get; set; }
    }
}