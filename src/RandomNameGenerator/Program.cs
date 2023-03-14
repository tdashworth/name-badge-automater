var names = Enumerable.Range(0, 50)
    //.Select(_ => $"{Faker.Name.First()} {Faker.Name.Last()}");
    // .Select(_ => $"{Faker.Name.First()} {Faker.Name.Middle()} {Faker.Name.Last()}");
    // .Select(_ => $"{Faker.Name.Middle()} {Faker.Name.Last()}, {Faker.Name.First()}");
    // .Select(_ => $"{Faker.Name.First().ToLower()}.{Faker.Name.Last().ToLower()}@{Faker.Internet.DomainName()}");
    // .Select(_ =>
    // {
    //   var first = Faker.Name.First();
    //   var last = Faker.Name.Last();

    //   return $"{first} {last} <{first.ToLower()}.{last.ToLower()}@{Faker.Internet.DomainName()}>";
    // });
    // .Select(_ =>
    //  {
    //    var first = Faker.Name.First();
    //    var last = Faker.Name.Last();

    //    return $"{last}, {first} <{first.ToLower()}.{last.ToLower()}@{Faker.Internet.DomainName()}>";
    //  })
     .Select(_ =>
     {
       var first = Faker.Name.First();
       var last = Faker.Name.Last();

       return $"{Faker.Name.Middle()} {last}, {first} <{first.ToLower()}.{last.ToLower()}@{Faker.Internet.DomainName()}>";
     })
    //  .OrderBy(_ => Faker.RandomNumber.Next())
     ;
// .Select(_ => $"{Faker.Name.First()} {Faker.Name.Last()}");
// .Select(_ => $"{Faker.Name.First()} {Faker.Name.Last()}");

foreach (var name in names)
{
  Console.WriteLine(name);
}