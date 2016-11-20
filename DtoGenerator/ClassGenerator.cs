using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DtoGenerator.DescriptionTypes;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.Options;
using TypeInterface;

namespace DtoGenerator
{
    internal class ClassGenerator
    {


        private List<ClassDescription> _classesList;

        private List<IType> _typesList;

        private string _namespace;

        private string _folderPath;

        private Workspace _workspace = new AdhocWorkspace();

        internal ClassGenerator(List<ClassDescription> classes, List<IType> types, string namespaceName, string outputFolder)
        {
            _classesList = classes;
            _typesList = types;
            _namespace = namespaceName;
            _folderPath = outputFolder;
        }

        internal void Generate()
        {
            foreach (var currentClass in _classesList)
            {
                var compilitionUnit = SyntaxFactory.CompilationUnit();
                var nameSpace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(_namespace));
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
                            SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(ReturnTypeName(property)),
                                property.Name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine($"Unknown type at property {property.Name}");
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
                compilitionUnit = compilitionUnit.AddMembers(nameSpace);
                WriteToFile(compilitionUnit, currentClass.ClassName);
            }
        }


        private string ReturnTypeName(PropertyDescription property)
        {
            return _typesList.Single(type => property.Format == type.Format).Type.ToString();
        }


        private void WriteToFile(CompilationUnitSyntax cu, string className)
        {
            SyntaxNode formattedNode = Formatter.Format(cu, _workspace);
            var writableString = formattedNode.ToFullString();
            File.WriteAllText($"{_folderPath}{Path.DirectorySeparatorChar}{className}.cs", writableString);
        }
    }
}