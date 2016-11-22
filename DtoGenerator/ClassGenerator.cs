using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DtoGenerator.DescriptionTypes;

namespace DtoGenerator
{
    internal class ClassGenerator: IDisposable
    {

        private readonly string _namespace;

        private readonly string _folderPath;

        private readonly SemaphoreSlim _semaphore;

        private readonly WaitHandle[] _manualResetEvent = new WaitHandle[DtoGenarator.ClassList.Count];

        internal ClassGenerator(string namespaceName, string outputFolder, int maxTaskCount)
        {
            _namespace = namespaceName;
            _folderPath = outputFolder;
            _semaphore = new SemaphoreSlim(maxTaskCount, maxTaskCount);
        }


        internal void Generate()
        {
            var eventCount = 0;
            foreach (var currentClass in DtoGenarator.ClassList)
            {
                _manualResetEvent[eventCount] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(GenerateClass, new object[] {currentClass, _manualResetEvent[eventCount]});
                eventCount++;
            }
        }


        private void GenerateClass(object methodParameter)
        {
            var paremeters = methodParameter as object[];
            if (paremeters != null)
            {
                var currentClass = paremeters[0] as ClassDescription;
                var resetEvent = paremeters[1] as ManualResetEvent;
                if (resetEvent == null || currentClass == null)
                {
                    return;
                }
                _semaphore.Wait();
                try
                {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} thread is running");
                    var compilitionUnit = SyntaxFactory.CompilationUnit();
                    var nameSpace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(_namespace));
                    {
                        var classCreation =
                            SyntaxFactory.ClassDeclaration(currentClass.ClassName)
                                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                    SyntaxFactory.Token(SyntaxKind.SealedKeyword)));
                        foreach (var property in currentClass.Proprties)
                        {
                            PropertyDeclarationSyntax generatedProperty;
                            try
                            {
                                generatedProperty =
                                    SyntaxFactory.PropertyDeclaration(
                                        SyntaxFactory.ParseTypeName(ReturnTypeName(property)),
                                        property.Name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                            }
                            catch (InvalidOperationException)
                            {
                                Console.WriteLine(
                                    $"Unknown type at property {property.Name}");
                                throw;
                            }
                            generatedProperty =
                                generatedProperty.AddAccessorListAccessors(
                                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
                            generatedProperty =
                                generatedProperty.AddAccessorListAccessors(
                                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
                            classCreation = classCreation.AddMembers(generatedProperty);
                        }
                        nameSpace = nameSpace.AddMembers(classCreation);
                    }
                    compilitionUnit = compilitionUnit.AddMembers(nameSpace);
                    WriteToFile(compilitionUnit, currentClass.ClassName);
                }
                catch
                {
                    Console.WriteLine($"Exception at thread {Thread.CurrentThread.ManagedThreadId}");
                }
                finally
                {
                    _semaphore.Release();
                    resetEvent.Set();
                }
            }
        }


        private string ReturnTypeName(PropertyDescription property)
        {
            return DtoGenarator.TypeList.Single(type => property.Format == type.Format).Type.ToString();
        }


        private void WriteToFile(CompilationUnitSyntax cu, string className)
        {
            SyntaxNode formattedNode = Formatter.Format(cu, new AdhocWorkspace());
            var writableString = formattedNode.ToFullString();
            File.WriteAllText($"{_folderPath}{Path.DirectorySeparatorChar}{className}.cs", writableString);
        }

        public void Dispose()
        {
            WaitHandle.WaitAll(_manualResetEvent);
            _semaphore.Dispose();
        }
    }
}