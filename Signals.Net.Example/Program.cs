using Signals.Net;

var quantity = new Signal<int>(1);
var basePrice = new Signal<int>(60);
var discountedPrice = new Signal<int>(20);

var toPay = new Computed<int>(() => quantity.Get() < 10 ? basePrice.Get() : discountedPrice.Get());


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