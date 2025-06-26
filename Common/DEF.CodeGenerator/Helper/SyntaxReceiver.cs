using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace DEF.CodeGenerator
{
#if LIB_3_11
    public class SyntaxReceiver : ISyntaxContextReceiver
#else
    public class SyntaxReceiver : ISyntaxReceiver
#endif
    {
        public List<IMethodSymbol> Methods { get; } = new List<IMethodSymbol>();
        public List<IFieldSymbol> Fields { get; } = new List<IFieldSymbol>();
        public List<IPropertySymbol> Properties { get; } = new List<IPropertySymbol>();
        public List<INamedTypeSymbol> Classes { get; } = new List<INamedTypeSymbol>();
        public List<INamedTypeSymbol> Interfaces { get; } = new List<INamedTypeSymbol>();

        public virtual bool CollectMethodSymbol { get; } = false;
        public virtual bool CollectFieldSymbol { get; } = false;
        public virtual bool CollectPropertySymbol { get; } = false;
        public virtual bool CollectClassSymbol { get; } = false;
        public virtual bool CollectInterfaceSymbol { get; } = false;

#if LIB_3_11
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            switch (context.Node)
            {
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    this.OnVisitMethodDeclaration(methodDeclarationSyntax, context.SemanticModel);
                    break;
                case PropertyDeclarationSyntax propertyDeclarationSyntax:
                    this.OnVisitPropertyDeclaration(propertyDeclarationSyntax, context.SemanticModel);
                    break;
                case FieldDeclarationSyntax fieldDeclarationSyntax:
                    this.OnVisitFieldDeclaration(fieldDeclarationSyntax, context.SemanticModel);
                    break;
                case ClassDeclarationSyntax classDeclarationSyntax:
                    this.OnVisitClassDeclaration(classDeclarationSyntax, context.SemanticModel);
                    break;
                case InterfaceDeclarationSyntax interfaceDeclarationSyntax:
                    this.OnVisitInterfaceDeclaration(interfaceDeclarationSyntax, context.SemanticModel);
                    break;
            };
        }
#else
        public List<SyntaxNode> SyntaxNodes { get; } = new List<SyntaxNode>();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            SyntaxNodes.Add(syntaxNode);
        }

        public void ParseGeneratorExecutionContext(GeneratorExecutionContext context)
        {

            foreach (var Node in SyntaxNodes)
            {
                var SemanticModel = context.Compilation.GetSemanticModel(Node.SyntaxTree);

                switch (Node)
                {
                    case MethodDeclarationSyntax methodDeclarationSyntax:
                        this.OnVisitMethodDeclaration(methodDeclarationSyntax, SemanticModel);
                        break;
                    case PropertyDeclarationSyntax propertyDeclarationSyntax:
                        this.OnVisitPropertyDeclaration(propertyDeclarationSyntax, SemanticModel);
                        break;
                    case FieldDeclarationSyntax fieldDeclarationSyntax:
                        this.OnVisitFieldDeclaration(fieldDeclarationSyntax, SemanticModel);
                        break;
                    case ClassDeclarationSyntax classDeclarationSyntax:
                        this.OnVisitClassDeclaration(classDeclarationSyntax, SemanticModel);
                        break;
                    case InterfaceDeclarationSyntax interfaceDeclarationSyntax:
                        this.OnVisitInterfaceDeclaration(interfaceDeclarationSyntax, SemanticModel);
                        break;
                };
            }
        }
#endif

        protected virtual void OnVisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, SemanticModel model)
        {
            if (!this.CollectMethodSymbol)
            {
                return;
            }

            if (!this.ShouldCollectMethodDeclaration(methodDeclarationSyntax))
            {
                return;
            }

            IMethodSymbol methodSymbol = model.GetDeclaredSymbol(methodDeclarationSyntax) as IMethodSymbol;
            if (methodSymbol is null)
            {
                return;
            }

            if (!this.ShouldCollectMethodSymbol(methodSymbol))
            {
                return;
            }

            this.Methods.Add(methodSymbol);
        }

        protected virtual bool ShouldCollectMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            return true;
        }

        protected virtual bool ShouldCollectMethodSymbol(IMethodSymbol methodSymbol)
        {
            return true;
        }

        protected virtual void OnVisitFieldDeclaration(FieldDeclarationSyntax fieldDeclarationSyntax, SemanticModel model)
        {
            if (!this.CollectFieldSymbol)
            {
                return;
            }

            if (!this.ShouldCollectFieldDeclaration(fieldDeclarationSyntax))
            {
                return;
            }

            IFieldSymbol fieldSymbol = model.GetDeclaredSymbol(fieldDeclarationSyntax) as IFieldSymbol;
            if (fieldSymbol == null)
            {
                return;
            }

            if (!this.ShouldCollectFieldSymbol(fieldSymbol))
            {
                return;
            }

            this.Fields.Add(fieldSymbol);
        }

        protected virtual bool ShouldCollectFieldDeclaration(FieldDeclarationSyntax fieldDeclarationSyntax)
        {
            return true;
        }

        protected virtual bool ShouldCollectFieldSymbol(IFieldSymbol fieldSymbol)
        {
            return true;
        }

        protected virtual void OnVisitPropertyDeclaration(PropertyDeclarationSyntax propertyDeclarationSyntax, SemanticModel model)
        {
            if (!this.CollectPropertySymbol)
            {
                return;
            }

            if (!this.ShouldCollectPropertyDeclaration(propertyDeclarationSyntax))
            {
                return;
            }

            IPropertySymbol propertySymbol = model.GetDeclaredSymbol(propertyDeclarationSyntax) as IPropertySymbol;
            if (propertySymbol == null)
            {
                return;
            }

            if (!this.ShouldCollectPropertySymbol(propertySymbol))
            {
                return;
            }

            this.Properties.Add(propertySymbol);
        }

        protected virtual bool ShouldCollectPropertyDeclaration(PropertyDeclarationSyntax propertyDeclarationSyntax)
        {
            return true;
        }

        protected virtual bool ShouldCollectPropertySymbol(IPropertySymbol propertySymbol)
        {
            return true;
        }

        protected virtual void OnVisitClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel model)
        {
            if (!this.CollectClassSymbol)
            {
                return;
            }

            if (!this.ShouldCollectClassDeclaration(classDeclarationSyntax))
            {
                return;
            }

            INamedTypeSymbol classSymbol = model.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
            if (classSymbol == null)
            {
                return;
            }

            if (!this.ShouldCollectClassSymbol(classSymbol))
            {
                return;
            }

            this.Classes.Add(classSymbol);
        }

        protected virtual bool ShouldCollectClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax)
        {
            return true;
        }

        protected virtual bool ShouldCollectClassSymbol(INamedTypeSymbol classSymbol)
        {
            return true;
        }

        protected virtual void OnVisitInterfaceDeclaration(InterfaceDeclarationSyntax interfaceDeclarationSyntax, SemanticModel model)
        {
            if (!this.CollectInterfaceSymbol)
            {
                return;
            }

            if (!this.ShouldCollectInterfaceDeclaration(interfaceDeclarationSyntax))
            {
                return;
            }

            INamedTypeSymbol interfaceSymbol = model.GetDeclaredSymbol(interfaceDeclarationSyntax) as INamedTypeSymbol;
            if (interfaceSymbol == null)
            {
                return;
            }

            if (!this.ShouldCollectInterfaceSymbol(interfaceSymbol))
            {
                return;
            }

            this.Interfaces.Add(interfaceSymbol);
        }

        protected virtual bool ShouldCollectInterfaceDeclaration(InterfaceDeclarationSyntax interfaceDeclarationSyntax)
        {
            return true;
        }

        protected virtual bool ShouldCollectInterfaceSymbol(INamedTypeSymbol interfaceSymbol)
        {
            return true;
        }
    }

}