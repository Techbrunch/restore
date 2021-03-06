﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Restore.ChangeResolution;
using Restore.Channel.Configuration;
using Restore.RxProto;

namespace Restore
{
    /// <summary>
    /// This is an interface with a rather complex set of generics. It is decided to facilitate a rather complex scenario
    /// and later on determine how to possibly simplify this or otherwise better compose different components.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TSynch"></typeparam>
    /// <remarks>
    /// TODO: Figure out a way to get rid of this insanely large generic signature. Because the config is passed around everywhere angle brackets explode.
    /// </remarks>
    public interface IChannelConfiguration<T1, T2, TId, TSynch> where TId : IEquatable<TId>
    {
        [Obsolete("Part of EndpointConfiguration now")]
        TypeConfiguration<T1, TId> Type1Configuration { get; }
        [Obsolete("Part of EndpointConfiguration now")]
        TypeConfiguration<T2, TId> Type2Configuration { get; }

        [NotNull]
        IEndpointConfiguration<T1, TId> Type1EndpointConfiguration { get; }
        [NotNull]
        IEndpointConfiguration<T2, TId> Type2EndpointConfiguration { get; }

        Func<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<TSynch>> ItemsPreprocessor { get; set; }

        /// <summary>
        /// Is primarily a part of Change Resolution. That's why the inteface is defined there.
        /// So this together with the SynchronizationResolvers is config only required in the Resolution step.
        /// </summary>
        [NotNull] IEnumerable<ISynchronizationResolver<TSynch>> SynchronizationResolvers { get; }

        /// <summary>
        /// Is primarily a part of Change Resolution. That's why the inteface is defined there.
        /// So this together with the SynchronizationResolvers is config only required in the Resolution step.
        /// </summary>
        ITranslator<T1, T2> TypeTranslator { get; }
    }
}
