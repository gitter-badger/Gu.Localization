﻿namespace Gu.Localization.Tests
{
    using System.Globalization;
    using System.Linq;

    using Gu.Localization;

    using NUnit.Framework;

    [Explicit("For some reason this is really slow in VS2015")]
    public class ResourceManagerWrapperTests
    {
        [TestCase("en", "English")]
        [TestCase("sv", "Svenska")]
        [TestCase("no", "So neutral")]
        public void Translate(string cultureName, string expected)
        {
            var manager = new ResourceManagerWrapper(Properties.Resources.ResourceManager);
            var cultureInfo = new CultureInfo(cultureName);
            var translated = manager.Translate(cultureInfo, nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual(expected, translated);
        }

        [Test]
        public void Languages()
        {
            var manager = new ResourceManagerWrapper(Properties.Resources.ResourceManager);
            var expected = new[] { "de", "en", "sv" };
            var actual = manager.Languages.Select(x => x.TwoLetterISOLanguageName)
                                .ToArray();
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
