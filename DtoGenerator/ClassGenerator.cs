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
    internal class ClassGenerator
    {

        private readonly string namespaceName;

        private readonly string folderPath;

        private readonly SemaphoreSlim semaphore;

        

        internal ClassGenerator(string namespaceName, string outputFolder, int maxTaskCount)
        {
            this.namespaceName = namespaceName;
            folderPath = outputFolder;
            semaphore = new SemaphoreSlim(maxTaskCount);
        }


        internal void Generate()
        {
            var eventCount = 0;
            foreach (var currentClass in DtoGenarator.ClassList)
            {
                semaphore.Wait();
                ThreadPool.QueueUserWorkItem(GenerateClass, new object[] {currentClass ,eventCount});
                eventCount++;
            }
        }


        private void GenerateClass(object methodParameter)
        {
            var paremeters = methodParameter as object[];
            try
            {
                if (paremeters != null)
                {
                    var currentClass = paremeters[0] as ClassDescription;
                    if (currentClass == null)
                    {
                        throw new NullReferenceException("Class is empty");
                    }
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} thread is running");
                    var compilationUnit = SyntaxFactory.CompilationUnit();
                    var nameSpace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(namespaceName));
                    
                    var classCreation =
                            SyntaxFactory.ClassDeclaration(currentClass.ClassName)
                                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                    SyntaxFactory.Token(SyntaxKind.SealedKeyword)));
                        foreach (var property in currentClass.Properties)
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
                                Console.WriteLine($"Exception at thread {Thread.CurrentThread.ManagedThreadId}");
                                continue;
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
                    compilationUnit = compilationUnit.AddMembers(nameSpace);
                    WriteToFile(compilationUnit, currentClass.ClassName);
                }
            }
            catch(InvalidOperationException)
            {
                Console.WriteLine($"Exception at thread {Thread.CurrentThread.ManagedThreadId}");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Exception at thread {Thread.CurrentThread.ManagedThreadId}");
            }
            finally
            {
                semaphore.Release();
            }
        }


        private string ReturnTypeName(PropertyDescription property)
        {              
            return DtoGenarator.TypeList.Single(t => (t.Name == property.Type) && (t.Format == property.Format)).Type.ToString(); 
        }


        private void WriteToFile(CompilationUnitSyntax cu, string className)
        {
            SyntaxNode formattedNode = Formatter.Format(cu, new AdhocWorkspace());
            var writableString = formattedNode.ToFullString();
            File.WriteAllText($"{folderPath}{Path.DirectorySeparatorChar}{className}.cs", writableString);
        }
    }
}