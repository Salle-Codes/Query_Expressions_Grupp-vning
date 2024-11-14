namespace Query_Expressions_Gruppövning
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

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
            InventoryFileGenerator inventoryFileGenerator = new InventoryFileGenerator();
            inventoryFileGenerator.GenerateInventoryFile("inventory.txt", 5000);
            var prod = LoadInventoryData();

            // Implementera query expressions här
            //Beräkna det totala värdet av alla produkter i lager
            decimal totalValue = inventory.Sum(x => x.Price);
            Console.WriteLine($"Totalt värde av produkter: {totalValue}");

            //Hitta alla produkter som inte har blivit påfyllda de senaste 30 dagarna
            var needsRestock = inventory.Where(x => x.LastRestocked.DayOfYear > 30);
            Console.WriteLine($"Alla produkter som inte har blivit påfyllda de senaste dagarna:");
            foreach (Product p in needsRestock)
            {
                Console.WriteLine($"{p.Name} fylldes senast på för {p.LastRestocked.DayOfYear} dagar sen");
            }

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
