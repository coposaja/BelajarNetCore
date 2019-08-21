using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Belajar4.Models
{
    public class AjaxValidationModel
    {
        public bool IsCool { get; set; }
        public int Age { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
