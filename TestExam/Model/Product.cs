//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestExam.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;

    public partial class Product
    {
        public Product()
        {
            this.OrderProduct = new HashSet<OrderProduct>();
        }
    
        public string ProductArticleNumber { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int ProductCategory { get; set; }
        public string ProductPhoto { get; set; }
        public string ProductImage { 
            get
            {
                if (File.Exists($"../../Resources/Products/{ProductPhoto}"))
                {
                    return $"../../Resources/Products/{ProductPhoto}";
                }
                else
                {
                    return $"../Resources/picture.png";
                }
            }
        }
        public string ProductManufacturer { get; set; }
        public decimal ProductCost { get; set; }
        public string ProductPrice
        {
            get
            {
                return $"Цена: {ProductCost:f2} р.";
            }
        }
        public Nullable<byte> ProductDiscountAmount { get; set; }
        public int ProductQuantityInStock { get; set; }
        public string ProductsLeft 
        { 
            get {
                if (ProductQuantityInStock > 0)
                    return $"Осталось {ProductQuantityInStock} шт.";
                else
                    return $"Нет на складе";
            } 
        }
        public string ProductStatus { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
    }
}
