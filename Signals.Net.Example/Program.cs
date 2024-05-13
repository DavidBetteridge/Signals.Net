﻿using Signals.Net;

var quantity = new ReadWriteSignal<int>(1);
var basePrice = new ReadWriteSignal<int>(60);
var discountedPrice = new ReadWriteSignal<int>(20);

var toPay = new Computed<int>(() => quantity.Get() < 10 ? 
                                        quantity.Get() * basePrice.Get() : 
                                        quantity.Get() * discountedPrice.Get());

Console.WriteLine(toPay.Get());
Console.ReadKey();

discountedPrice.Set(10);
Console.ReadKey();

Console.WriteLine(toPay.Get());
Console.ReadKey();

quantity.Set(11);
Console.ReadKey();

Console.WriteLine(toPay.Get());
Console.ReadKey();