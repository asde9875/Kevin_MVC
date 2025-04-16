using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Models.Entities
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCartEntity> ShoppingCartList { get; set; }

        public OrderHeader OrderHeader { get; set; }
        //public double OrderTotal { get; set; }
    }
}
