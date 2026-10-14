
using System;
using Project_OOP;
using Project_OOP.AProduct;
using ProjectOOP_TokoOnline;

Member[] registeredMembers = new Member[2];

registeredMembers[0] = new Member("Andi", "Jakarta");
registeredMembers[1] = new Member("Budi", "Bandung");

Product[] catalog = new Product[4];

catalog[0] = new Electronic(
    "EL001", "Mouse", 100000m, 10, false
);

catalog[1] = new Electronic(
    "EL002", "Keyboard", 300000m, 5, true
);

catalog[2] = new Outfit(
    "OT001", "T-Shirt", 80000m, 20, true
);

catalog[3] = new Food(
    "FD001",
    "Bread",
    15000m,
    8,
    DateTime.Today.AddDays(5)
);

Menu menu = new Menu(registeredMembers, catalog);
menu.Start();
