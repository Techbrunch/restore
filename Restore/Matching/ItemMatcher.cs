using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Restore.Channel.Configuration;
using Restore.Extensions;

namespace Restore.Matching
{
    public class ItemMatcher<T1, T2, TId, TSynch> where TId : IEquatable<TId>
    {
        [NotNull] readonly TypeConfiguration<T1, TId> _t1Config;
        [NotNull] readonly TypeConfiguration<T2, TId> _t2Config;

        private ItemMatcher(
            [NotNull] TypeConfiguration<T1, TId> t1Config,
            [NotNull] TypeConfiguration<T2, TId> t2Config)
        {
            if (t1Config == null) throw new ArgumentNullException(nameof(t1Config));
            if (t2Config == null) throw new ArgumentNullException(nameof(t2Config));

            _t1Config = t1Config;
            _t2Config = t2Config;
        }

        public ItemMatcher([NotNull] ChannelConfiguration<T1, T2, TId, TSynch> channelConfig) : this(channelConfig.Type1Configuration, channelConfig.Type2Configuration)
        {
            if (channelConfig == null) throw new ArgumentNullException(nameof(channelConfig));

            ChannelConfig = channelConfig;
        }

        [NotNull]
        public ChannelConfiguration<T1, T2, TId, TSynch> ChannelConfig { get; }

        [NotNull]
        public IEnumerable<ItemMatch<T1, T2>> Match(
            [NotNull] IEnumerable<T1> result1,
            [NotNull] IEnumerable<T2> result2)
        {
            // You're probably doing something wrong when either of the lists is null. Rather not obfuscate that by creating an empty list myself.
            if (result1 == null) throw new ArgumentNullException(nameof(result1));
            if (result2 == null) throw new ArgumentNullException(nameof(result2));

            // The disadvantage is that this can be blocking countrary to IObservable.
            var result1List = result1.ToList();
            var result2List = result2.ToList();

            // I can not use this because it can extract a null id. In that case I should just take the item. Just returning a negative Id in
            // that case also doesn't give me the desired result. Because they all need to be different id's in that case.

            foreach (T1 item1 in result1List)
            {
                var item1Id = _t1Config.IdExtractor(item1);
                if (item1Id == null)
                {
                    // No id to match on could be extracted, it's same to assume it can not be matched because it still requires synchronization.
                    yield return new ItemMatch<T1, T2>(item1, default(T2));
                    continue;
                }

                //var item2Match = result2List.FirstOrDefault(item2 => _t1Config.IdExtractor(item1).Equals(item1Id));
                var item2Match = result2List.Extract(item2 => _t2Config.IdExtractor(item2).Equals(item1Id));
                if (EqualityComparer<T2>.Default.Equals(item2Match, default(T2)))
                {
                    yield return new ItemMatch<T1, T2>(item1, default(T2));
                }
                else
                {
                    yield return new ItemMatch<T1, T2>(item1, item2Match);
                }
            }

            foreach (var item2 in result2List)
            {
                yield return new ItemMatch<T1, T2>(default(T1), item2);
            }
        }
    }
}