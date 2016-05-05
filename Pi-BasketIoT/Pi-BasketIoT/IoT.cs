using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi_BasketIoT
{
    public class IoT
    {
        [Key]
        public int Id { get; set; }
        public int Score { get; set; }
    }
}
