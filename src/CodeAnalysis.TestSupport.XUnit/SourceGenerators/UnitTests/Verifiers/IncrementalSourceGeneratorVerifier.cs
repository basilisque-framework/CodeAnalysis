/*
   Copyright 2023 Alexander Stärk

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Basilisque.CodeAnalysis.TestSupport.XUnit.SourceGenerators.UnitTests.Verifiers
{
    /// <summary>
    /// Helper class to support verifying incremental source generators with MSTest
    /// </summary>
    /// <typeparam name="TSourceGenerator">The incremental source generator to be verified</typeparam>
    public class IncrementalSourceGeneratorVerifier<TSourceGenerator> : Basilisque.CodeAnalysis.TestSupport.SourceGenerators.UnitTests.Verifiers.IncrementalSourceGeneratorVerifier<TSourceGenerator, XUnitVerifier>
        where TSourceGenerator : IIncrementalGenerator, new()
    { }
}
