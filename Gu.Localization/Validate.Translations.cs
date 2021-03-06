﻿namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Resources;

    using Gu.Localization.Errors;

    /// <summary>
    /// This class is meant to be used in unit tests.
    /// Contains helper methods for asserting properties on localization.
    /// </summary>
    public static partial class Validate
    {
        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) All keys in <paramref name="resourceManager"/> have non null values for all cultures in <see cref="Translator.Cultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The resource managerr to check</param>
        /// <returns>An <see cref="TranslationErrors"/> with all errors found in <paramref name="resourceManager"/></returns>
        public static TranslationErrors Translations(ResourceManager resourceManager)
        {
            return Translations(resourceManager, Translator.Cultures.Prepend(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) All keys in <paramref name="resourceManager"/> have non null values for all cultures in <paramref name="cultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The resource managerr to check</param>
        /// <param name="cultures">The cultures to check resources for</param>
        /// <returns>An <see cref="TranslationErrors"/> with all errors found in <paramref name="resourceManager"/></returns>
        public static TranslationErrors Translations(ResourceManager resourceManager, IEnumerable<CultureInfo> cultures)
        {
            using (var clone = resourceManager.Clone())
            {
                var resources = GetResources(clone, cultures);
                var keys = GetKeys(resourceManager);
                Dictionary<string, IReadOnlyList<TranslationError>> errors = null;
                foreach (var key in keys)
                {
                    var keyErrors = Translations(resources, key);
                    if (keyErrors.Count == 0)
                    {
                        continue;
                    }

                    if (errors == null)
                    {
                        errors = new Dictionary<string, IReadOnlyList<TranslationError>>();
                    }

                    errors.Add(key, keyErrors);
                }

                return errors == null
                           ? TranslationErrors.Empty
                           : new TranslationErrors(errors);
            }
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that all members of <typeparamref name="T"/> have corresponding key in <paramref name="resourceManager"/>
        /// and that the key has a non null value for all cultures in <see cref="Translator.Cultures"/>
        /// </summary>
        /// <typeparam name="T">An enum type</typeparam>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <typeparamref name="T"/></param>
        /// <returns>A list with all members that does not have </returns>
        public static TranslationErrors EnumTranslations<T>(ResourceManager resourceManager)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return EnumTranslations<T>(resourceManager, Translator.Cultures.Prepend(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that all members of <typeparamref name="T"/> have corresponding key in <paramref name="resourceManager"/>
        /// and that the key has a non null value for all cultures in <see cref="Translator.Cultures"/>
        /// </summary>
        /// <typeparam name="T">An enum type</typeparam>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <typeparamref name="T"/></param>
        /// <param name="cultures">The cultures to check for.</param>
        /// <returns>A list with all members that does not have </returns>
        public static TranslationErrors EnumTranslations<T>(ResourceManager resourceManager, IEnumerable<CultureInfo> cultures)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            using (var clone = resourceManager.Clone())
            {
                var resources = GetResources(clone, cultures);
                Dictionary<string, IReadOnlyList<TranslationError>> errors = null;
                foreach (var key in Enum.GetNames(typeof(T)))
                {
                    var keyErrors = Translations(resources, key);
                    if (keyErrors.Count == 0)
                    {
                        continue;
                    }

                    if (errors == null)
                    {
                        errors = new Dictionary<string, IReadOnlyList<TranslationError>>();
                    }

                    errors.Add(key, keyErrors);
                }

                return errors == null
                           ? TranslationErrors.Empty
                           : new TranslationErrors(errors);
            }
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) <paramref name="key"/> has non null values for all cultures in <see cref="Translator.Cultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="key"/></param>
        /// <param name="key">The key</param>
        /// <returns>A list with all errors for the key or an empty list if no errors.</returns>
        public static IReadOnlyList<TranslationError> Translations(ResourceManager resourceManager, string key)
        {
            return Translations(resourceManager, key, Translator.Cultures.Prepend(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) <paramref name="key"/> has non null values for all cultures in <see cref="Translator.Cultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="key"/></param>
        /// <param name="key">The key</param>
        /// <param name="cultures">The cultures to check</param>
        /// <returns>A list with all errors for the key or an empty list if no errors.</returns>
        public static IReadOnlyList<TranslationError> Translations(ResourceManager resourceManager, string key, IEnumerable<CultureInfo> cultures)
        {
            using (var clone = resourceManager.Clone())
            {
                var resources = GetResources(clone, cultures);
                return Translations(resources, key);
            }
        }

        private static IReadOnlyList<TranslationError> Translations(IReadOnlyDictionary<CultureInfo, ResourceSet> resources, string key)
        {
            // not optimized at all here, only expecting this to be called in tests.
            List<TranslationError> errors = new List<TranslationError>();
            var translations = resources.ToDictionary(x => x.Key, x => x.Value.GetString(key));
            FormatError formatErrors;
            if (TryGetFormatErrors(key, translations, out formatErrors))
            {
                errors.Add(formatErrors);
            }

            if (translations.Any(x => x.Value == null))
            {
                errors.Add(new MissingTranslation(key, translations.Where(x => x.Value == null).Select(x => x.Key).ToArray()));
            }

            return errors;
        }

        private static bool TryGetFormatErrors(string key, IReadOnlyDictionary<CultureInfo, string> translations, out FormatError formatErrors)
        {
            int? count = null;
            foreach (var translation in translations.Values)
            {
                int indexCount;
                bool? anyItemHasFormat;
                if (!FormatString.IsValidFormat(translation, out indexCount, out anyItemHasFormat))
                {
                    formatErrors = new FormatError(key, translations);
                    return true;
                }

                if (count == null)
                {
                    count = indexCount;
                    continue;
                }

                if (count != indexCount)
                {
                    formatErrors = new FormatError(key, translations);
                    return true;
                }
            }

            formatErrors = null;
            return false;
        }

        private static IReadOnlyList<string> GetKeys(ResourceManager resourceManager)
        {
            return resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false)
                             .OfType<DictionaryEntry>()
                             .Select(x => x.Key)
                             .OfType<string>()
                             .ToArray();
        }

        private static IReadOnlyDictionary<CultureInfo, ResourceSet> GetResources(ResourceManagerExt.ResourceManagerClone clone, IEnumerable<CultureInfo> cultures)
        {
            if (clone == null || clone.ResourceManager == null)
            {
                return EmptyReadOnlyDictionary<CultureInfo, ResourceSet>.Default;
            }

            return cultures.ToDictionary(c => c, c => clone.ResourceManager.GetResourceSet(c, true, false), CultureInfoComparer.Default);
        }
    }
}
