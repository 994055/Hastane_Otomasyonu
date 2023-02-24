using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hastane.Models.Entity;

namespace Hastane.Models
{
    public class ViewModels
    {

        public IEnumerable<SelectListItem> iller { get; set; }
        public IEnumerable<SelectListItem> hastaneler { get; set; }
        public IEnumerable<SelectListItem> saat { get; set; }
 

    }
}