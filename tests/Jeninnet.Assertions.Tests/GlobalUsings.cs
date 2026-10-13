//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Globalization;
global using System.Linq.Expressions;
global using System.Net;
global using System.Net.Http;
global using System.Reflection;
global using System.Reflection.Emit;
global using System.Text;
global using System.Text.Json.Nodes;
global using System.Text.RegularExpressions;
global using System.Xml;
global using System.Xml.Linq;
global using Jeninnet.Assertions;
global using Jeninnet.Assertions.Equivalency;
global using Jeninnet.Assertions.Events;
global using Jeninnet.Assertions.Extensions;
global using Jeninnet.Assertions.IO;
global using Jeninnet.Assertions.Localization;
global using Jeninnet.Assertions.Tests.EquivalencyFixtures;
global using Jeninnet.Assertions.Tests.LocalizationFixtures;
global using Jeninnet.Assertions.Tests.SelectorFixtures;
global using Jeninnet.Assertions.Types;
global using Jeninnet.Assertions.Utilities;
global using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: AssertionEngineInitializer(typeof(Jeninnet.Assertions.Tests.TestInitializer), nameof(Jeninnet.Assertions.Tests.TestInitializer.Initialize))]
