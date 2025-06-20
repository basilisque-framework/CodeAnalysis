﻿/*
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
using BenchmarkDotNet.Running;

namespace Basilisque.CodeAnalysis.Benchmarks;

internal static class Program
{
    static void Main(string[] args)
    {
        BenchmarkDotNet.Configs.IConfig? config;
#if DEBUG
        config = new BenchmarkDotNet.Configs.DebugInProcessConfig();
#else
        config = null;
#endif
        BenchmarkRunner.Run(typeof(Program).Assembly, config);
    }
}