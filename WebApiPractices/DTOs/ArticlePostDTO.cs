using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class ArticlePostDTO
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal SalePrice { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
