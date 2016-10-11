﻿using DtoGenerator.DtoDescriptor;
using DtoGenerator.CodeGenerators.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using DtoGenerator.CodeGenerators.GeneratedItems;
using System;
using System.Threading;

namespace DtoGenerator.CodeGenerators
{
    internal class CSCodeGenerator : ICodeGenerator
    {
        private SupportedTypesTable supportedTypes = new SupportedTypesTable();
        private string classNamespace;
        private ClassDescription classDescription;
        private Semaphore semaphore; 

        public CSCodeGenerator(int maxThreadNumber)
        {
            if (maxThreadNumber < 1) throw new ArgumentOutOfRangeException(nameof(maxThreadNumber));
            semaphore = new Semaphore(maxThreadNumber, maxThreadNumber);
            var _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);
        }

        public void GenerateCode(object threadContext)
        {
            try
            {
                semaphore.WaitOne();
                if(threadContext == null) throw new ArgumentNullException(nameof(threadContext));
                if (threadContext.GetType() != typeof(TaskInfo)) throw new ArgumentOutOfRangeException(nameof(threadContext));

                PerformTask(threadContext as TaskInfo);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private void PerformTask(TaskInfo parameters)
        {
            classNamespace = parameters.ClassesNamespace;
            classDescription = parameters.ClassDescription;

            NamespaceDeclarationSyntax namespaceDeclaration = GenerateNamespace();
            GeneratedClass generatedClass = new GeneratedClass(classDescription.ClassName, Formatter.Format(namespaceDeclaration, new AdhocWorkspace()).ToFullString());

            parameters.result = generatedClass;
            parameters.ResetEvent.Set();
        }


        private NamespaceDeclarationSyntax GenerateNamespace()
        {
            NamespaceDeclarationSyntax namespaceDeclaration = NamespaceDeclaration(IdentifierName(classNamespace));
            ClassDeclarationSyntax classDeclaration = GenerateClass(classDescription.ClassName, classDescription.Properties);
            namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);

            return namespaceDeclaration;
        }

        private ClassDeclarationSyntax GenerateClass(string className, Property[] properties)
        {
            ClassDeclarationSyntax classDeclaration = ClassDeclaration(className);
            classDeclaration = classDeclaration.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));

            MemberDeclarationSyntax[] propertiesDeclarations = new MemberDeclarationSyntax[properties.Length];
            for(int i = 0; i < properties.Length; i++)
            {
                Property property = properties[i];
                string propertyType = supportedTypes.GetNetType(property.Type, property.Format);
                propertiesDeclarations[i] = GenerateProperty(property.Name, propertyType);
                
            }

            classDeclaration = classDeclaration.AddMembers(propertiesDeclarations);
            return classDeclaration;
        }

        private PropertyDeclarationSyntax GenerateProperty(string propertyName, string propertyType)
        {
            PropertyDeclarationSyntax propertyDeclaration = PropertyDeclaration(IdentifierName(propertyType), propertyName);
            propertyDeclaration = propertyDeclaration.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
            propertyDeclaration = propertyDeclaration.WithAccessorList(
                AccessorList(
                    List(
                        new[] {
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        }
                        )
                    )
                );

            return propertyDeclaration;
        }
    }
}
