namespace Query_Expressions_Gruppövning
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;

    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime LastRestocked { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Category: {Category}, Quantity: {Quantity}, Price: {Price:C}, Last Restocked: {LastRestocked:d}";
        }
    }

    class Program
    {
        static List<Product> inventory;

        static void Main(string[] args)
        {
            int amount = 0;
            InventoryFileGenerator inventoryFileGenerator = new InventoryFileGenerator();
            inventoryFileGenerator.GenerateInventoryFile("inventory.txt", 5000);
            var prod = LoadInventoryData();

            // Implementera query expressions här
            //Lista alla produkter i kategorin "Verktyg" sorterade efter pris(stigande).
            var products = from p in inventory
                           where p.Category == "Verktyg"
                           orderby p.Price ascending
                           select p;
            foreach (var product in products)
            {
                Console.WriteLine($"Produkt: {product.Name} kostar {product.Price}Kr");
            }
            //Gruppera produkterna efter kategori och visa antalet produkter i varje kategori. 
            var grouping = from p in inventory
                           group p by p.Category into katGroup

                           orderby katGroup.Key

                           select new
                           {
                               katName = katGroup.Key,
                               produkter = katGroup.ToList(),
                               TotalQuantity = katGroup.Sum(x => x.Quantity)
                           };
            //***Hitta de 5 produkter som har lägst lagersaldo och behöver beställas***

            var productsToOrder = (from product in inventory  
                                   where product.Quantity < 1001
                                   orderby product.Quantity ascending 
                                   select product).Take(5);

            foreach (var product in productsToOrder)
            {
                Console.WriteLine($"Product: {product.Name}, Quantity: {product.Quantity}");
            }
            

            //***Öka priset med 10% för alla produkter i kategorin "Elektronik"***
            var electronicsProducts = prod.Where(p => p.Category == "Elektronik");   //sorterar ut all Elektronik från prod(List<produkter>) och lägger i electronicsProduct.

            foreach (var product in electronicsProducts)                             // för varje prudukt/sak i electronicsProduct
            {
                product.Price *= 1.10m;                                              //ökar priset med 10% på alla produkter i electronicsProduct
            }
            Console.WriteLine("Produkter i kategorin Elektronik efter prisökningen:");
            foreach (var product in electronicsProducts)
            {
                Console.WriteLine(product.ToString());
            }

            foreach (var catName in grouping)
            {
                Console.WriteLine(catName.katName);
                Console.WriteLine($"Antal produkter: {catName.TotalQuantity}");
            }
            //Hitta den kategori som har det högsta genomsnittliga priset per produkt. 
            var priceMax = (from p in inventory
                            group p by p.Category into priceCat
                            select priceCat.Average(x => x.Price)).Max();
                           
            Console.WriteLine($"{priceMax:C2}");
            Console.ReadLine();
        }

        static List<Product> LoadInventoryData()
        {
            string[] lines = File.ReadAllLines("inventory.txt");
            inventory = lines.Skip(1) // Hoppa över rubrikraden
                            .Select(line =>
                            {
                                var parts = line.Split(',');
                                return new Product
                                {
                                    Id = int.Parse(parts[0]),
                                    Name = parts[1],
                                    Category = parts[2],
                                    Quantity = int.Parse(parts[3]),
                                    Price = decimal.Parse(parts[4], CultureInfo.InvariantCulture),
                                    LastRestocked = DateTime.ParseExact(parts[5], "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                };
                            }).ToList();
            return inventory;
        }

    }
}
