// <copyright file="PexAssemblyInfo.cs">Copyright ©  2017</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("SolarSystem.Test")]
[assembly: PexInstrumentAssembly("System.Core")]
[assembly: PexInstrumentAssembly("xunit.assert")]
[assembly: PexInstrumentAssembly("SolarSystem")]
[assembly: PexInstrumentAssembly("Ploeh.AutoFixture")]
[assembly: PexInstrumentAssembly("xunit.core")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "xunit.assert")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "SolarSystem")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Ploeh.AutoFixture")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "xunit.core")]

