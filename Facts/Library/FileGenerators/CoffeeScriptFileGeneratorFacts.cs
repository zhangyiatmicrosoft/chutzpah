﻿using System.IO;
using Chutzpah.FileGenerator;
using Chutzpah.FileGenerators;
using Chutzpah.Models;
using Chutzpah.Utility;
using Chutzpah.Wrappers;
using Moq;
using Xunit;

namespace Chutzpah.Facts
{
    public class CoffeeScriptFileGeneratorFacts
    {
        private class TestableCoffeeScriptFileGenerator : Testable<CoffeeScriptFileGenerator>
        {
            public TestableCoffeeScriptFileGenerator()
            {
            }
        }

        public class CanHandleFile
        {
            [Fact]
            public void Will_return_false_for_non_coffee_script_files()
            {
                var converter = new TestableCoffeeScriptFileGenerator();
                var file = new ReferencedFile { Path = "somePath.js" };

                var result = converter.ClassUnderTest.CanHandleFile(file);

                Assert.False(result);
            }

            [Fact]
            public void Will_return_true_for_coffee_script_files()
            {
                var converter = new TestableCoffeeScriptFileGenerator();
                var file = new ReferencedFile { Path = "somePath.coffee" };

                var result = converter.ClassUnderTest.CanHandleFile(file);

                Assert.True(result);
            }
        }


        public class GenerateCompiledSources
        {
            [Fact]
            public void Will_return_compiled_files_and_set_to_cache()
            {
                var generator = new TestableCoffeeScriptFileGenerator();
                var file = new ReferencedFile { Path = "path1.ts" };
                generator.Mock<ICoffeeScriptEngineWrapper>().Setup(x => x.Compile(It.IsAny<string>())).Returns("compiled");
                generator.Mock<IFileSystemWrapper>().Setup(x => x.GetText(It.IsAny<string>())).Returns("content");
                generator.Mock<ICompilerCache>().Setup(x => x.Get(It.IsAny<string>())).Returns((string)null);

                var result = generator.ClassUnderTest.GenerateCompiledSources(new[] { file });

                Assert.Equal("compiled", result[file.Path]);
                generator.Mock<ICompilerCache>().Verify(x => x.Set("content", "compiled"));
            }

            [Fact]
            public void Will_return_compiled_files_that_are_cache()
            {
                var generator = new TestableCoffeeScriptFileGenerator();
                var file = new ReferencedFile { Path = "path1.ts" };
                generator.Mock<IFileSystemWrapper>().Setup(x => x.GetText(It.IsAny<string>())).Returns("content");
                generator.Mock<ICompilerCache>().Setup(x => x.Get(It.IsAny<string>())).Returns("compiled");

                var result = generator.ClassUnderTest.GenerateCompiledSources(new[] { file });

                Assert.Equal("compiled", result[file.Path]);
                generator.Mock<ICoffeeScriptEngineWrapper>().Verify(x => x.Compile(It.IsAny<string>()), Times.Never());
                generator.Mock<ICompilerCache>().Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            }
        }
    }
}