using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Owin_Auth.Id
{
    public class UserValidation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ValidationId { get; set; }

        public string Username { get; set; }
        public string LongId { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
