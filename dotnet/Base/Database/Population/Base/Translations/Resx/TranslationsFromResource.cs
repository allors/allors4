﻿namespace Allors.Database.Population.Resx
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using Database.Meta;
    using Population;

    public partial class TranslationsFromResource
    {
        private const string Translations = ".translations.";
        private const string ResourcesExtension = ".resources";

        public TranslationsFromResource(IMetaPopulation metaPopulation, ITranslationConfiguration configuration)
        {
            CultureInfo[] cultureInfos = [CultureInfo.InvariantCulture, .. configuration.AdditionalCultureInfos];

            var assembly = Assembly.GetExecutingAssembly();

            this.ResourceSetByCultureInfoByRoleTypeByClass = new Dictionary<IClass, IDictionary<IRoleType, IDictionary<CultureInfo, ResourceSet>>>();

            foreach ((String baseName, IClass @class, IRoleType roleType) in assembly.GetManifestResourceNames()
                .Where(v => v.Contains(Translations, StringComparison.OrdinalIgnoreCase) && v.EndsWith(ResourcesExtension, StringComparison.OrdinalIgnoreCase))
                .Select(v =>
                {
                    var end = v.LastIndexOf('.') + 1;
                    var middle = v.LastIndexOf('.', end - 2) + 1;
                    var begin = v.LastIndexOf('.', middle - 2) + 1;

                    var baseName = v.Substring(0, end - 1);
                    var className = v.Substring(begin, middle - begin - 1);
                    var roleName = v.Substring(middle, end - middle - 1);

                    var @class = metaPopulation.Classes.First(w => w.SingularName.Equals(className, StringComparison.OrdinalIgnoreCase));
                    var roleType = @class.RoleTypes.First(w => w.SingularName.Equals(roleName, StringComparison.OrdinalIgnoreCase));

                    return new Tuple<String, IClass, IRoleType>(baseName, @class, roleType);
                }))
            {
                if (!this.ResourceSetByCultureInfoByRoleTypeByClass.TryGetValue(@class, out var resourceSetByCultureInfoByRoleType))
                {
                    resourceSetByCultureInfoByRoleType = new Dictionary<IRoleType, IDictionary<CultureInfo, ResourceSet>>();
                    this.ResourceSetByCultureInfoByRoleTypeByClass.Add(@class, resourceSetByCultureInfoByRoleType);
                }

                var resourceManager = new ResourceManager(baseName, assembly);
                var resourceSetByCultureInfo = cultureInfos.ToDictionary(
                    cultureInfo => Equals(CultureInfo.InvariantCulture, cultureInfo)
                        ? configuration.DefaultCultureInfo
                        : cultureInfo,
                    cultureInfo => resourceManager.GetResourceSet(cultureInfo, true, false));
                resourceSetByCultureInfoByRoleType.Add(roleType, resourceSetByCultureInfo);
            }
        }

        public IDictionary<IClass, IDictionary<IRoleType, IDictionary<CultureInfo, ResourceSet>>> ResourceSetByCultureInfoByRoleTypeByClass { get; }
    }
}