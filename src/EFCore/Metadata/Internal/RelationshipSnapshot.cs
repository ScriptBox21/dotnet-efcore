// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

#nullable enable

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class RelationshipSnapshot
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public RelationshipSnapshot(
            [NotNull] InternalForeignKeyBuilder relationship,
            [CanBeNull] EntityType.Snapshot? ownedEntityTypeSnapshot,
            [CanBeNull] List<(SkipNavigation, ConfigurationSource)>? referencingSkipNavigations)
        {
            Relationship = relationship;
            OwnedEntityTypeSnapshot = ownedEntityTypeSnapshot;
            ReferencingSkipNavigations = referencingSkipNavigations;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalForeignKeyBuilder Relationship { [DebuggerStepThrough] get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual EntityType.Snapshot? OwnedEntityTypeSnapshot { [DebuggerStepThrough] get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual List<(SkipNavigation SkipNavigation, ConfigurationSource ForeignKeyConfigurationSource)>? ReferencingSkipNavigations
        {
            [DebuggerStepThrough] get;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalForeignKeyBuilder? Attach([CanBeNull] InternalEntityTypeBuilder? entityTypeBuilder = null)
        {
            entityTypeBuilder ??= Relationship.Metadata.DeclaringEntityType.Builder;

            var newRelationship = Relationship.Attach(entityTypeBuilder);
            if (newRelationship != null)
            {
                OwnedEntityTypeSnapshot?.Attach(
                    newRelationship.Metadata.ResolveOtherEntityType(entityTypeBuilder.Metadata).Builder);

                if (ReferencingSkipNavigations != null)
                {
                    foreach (var referencingNavigationTuple in ReferencingSkipNavigations)
                    {
                        var skipNavigation = referencingNavigationTuple.SkipNavigation;
                        if (!skipNavigation.IsInModel)
                        {
                            var navigationEntityType = skipNavigation.DeclaringEntityType;
                            skipNavigation = !navigationEntityType.IsInModel
                                ? null
                                : navigationEntityType.FindSkipNavigation(skipNavigation.Name);
                        }

                        skipNavigation?.Builder.HasForeignKey(
                            newRelationship.Metadata, referencingNavigationTuple.ForeignKeyConfigurationSource);
                    }
                }
            }

            return newRelationship;
        }
    }
}
