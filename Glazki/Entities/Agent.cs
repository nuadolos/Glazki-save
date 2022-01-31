//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Glazki.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Agent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Agent()
        {
            this.AgentPriorityHistory = new HashSet<AgentPriorityHistory>();
            this.ProductSale = new HashSet<ProductSale>();
            this.Shop = new HashSet<Shop>();
        }
    
        public int ID { get; set; }
        public string Title { get; set; }
        public int AgentTypeID { get; set; }
        public string Address { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string DirectorName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public int Priority { get; set; }

        public string LogoPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Logo))
                    return null;
                else
                    return Logo;
            }
        }
        public int ProductCount
        { 
            get
            {
                DateTime now = DateTime.Now;
                int count = 0;

                foreach(var prodSale in ProductSale)
                {
                    if (prodSale.SaleDate.AddYears(1) > now)
                        count += prodSale.ProductCount;
                }

                return count;
            }
        }
        public int Discount
        {
            get
            {
                decimal sum = 0;
                int discont;

                foreach (ProductSale prodSale in ProductSale)
                {
                    sum += prodSale.ProductCount * prodSale.Product.MinCostForAgent;
                }

                if (sum >= 0 && sum <= 10000)
                    discont = 0;
                else if (sum > 10000 && sum <= 50000)
                    discont = 5;
                else if (sum > 50000 && sum <= 150000)
                    discont = 10;
                else if (sum > 150000 && sum <= 500000)
                    discont = 20;
                else
                    discont = 25;

                return discont;
            }
        }


        public virtual AgentType AgentType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AgentPriorityHistory> AgentPriorityHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductSale> ProductSale { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Shop> Shop { get; set; }
    }
}
