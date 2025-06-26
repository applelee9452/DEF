using Microsoft.CodeAnalysis;

namespace DEF.CodeGenerator
{
    public class MethodsWithAttributeReceiver : SyntaxReceiver
    {
        private string expectedAttribute;
        public MethodsWithAttributeReceiver(string expectedAttribute) => this.expectedAttribute = expectedAttribute;

        public override bool CollectMethodSymbol { get; } = true;

        protected override bool ShouldCollectMethodSymbol(IMethodSymbol methodSymbol)
            => methodSymbol.HasAttribute(this.expectedAttribute);
    }
}