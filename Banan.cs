using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp40
{
    [Table("tblBanans")]
    public class Banan
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string FirstName { get; set; } = String.Empty;
        [Required, StringLength(100)]
        public string LastName { get; set; } = String.Empty;

        [StringLength(200)]
        public string? Image { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public bool Sex { get; set; }
    }
}
