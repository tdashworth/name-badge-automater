namespace NameBadgeAutomater
{
    public static class CollectionExtensions
    {
        public static IEnumerable<TResult> ZipLongest<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> func,
            TFirst padder1 = default!,
            TSecond padder2 = default!)
        {
            var firstExp = first.Concat(
                Enumerable.Repeat(
                    padder1,
                    Math.Max(second.Count() - first.Count(), 0)
                )
            );
            var secExp = second.Concat(
                Enumerable.Repeat(
                    padder2,
                    Math.Max(first.Count() - second.Count(), 0)
                )
            );
            return firstExp.Zip(secExp, (a, b) => func(a, b));
        }
    }
}