using System;
using Project_OOP;
using Project_OOP.AProduct;
using ProjectOOP_TokoOnline;

// Registered members (preloaded data).
Member[] registeredMembers = new Member[2];

registeredMembers[0] = new Member("Andi", "Jakarta");
registeredMembers[1] = new Member("Budi", "Bandung");

// Product catalog (preloaded data). Building a product validates its
// price (> 0) and stock (>= 0); any bad data prints a validation message.
Product[] catalog;

try
{
    catalog = new Product[]
    {
        // Electronics: guarantee rate (0.05 = +5% warranty, 0 = none).
        new Electronic("EL001", "Mouse", 100000m, 10, 0m),
        new Electronic("EL002", "Keyboard", 300000m, 5, 0.05m),

        // Outfits: isOnPromo (true applies the 20% discount).
        new Outfit("OT001", "T-Shirt", 80000m, 20, true),
        new Outfit("OT002", "Jacket", 200000m, 10, false),

        // Food: days until expired (0 or less = expired, cannot be bought).
        new Food("FD001", "Bread", 15000m, 8, 5),
        new Food("FD002", "Milk", 20000m, 6, 0),
    };
}
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message);
    return;
}

Menu menu = new Menu(registeredMembers, catalog);
menu.Start();
