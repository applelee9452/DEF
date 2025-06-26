using Microsoft.CodeAnalysis;

namespace DEF.CodeGenerator
{
    public class ClassesWithInterfacesReceiver : SyntaxReceiver
    {
        private string implementedInterface;
        public ClassesWithInterfacesReceiver(string implementedInterface) => this.implementedInterface = implementedInterface;

        public override bool CollectClassSymbol { get; } = true;

        protected override bool ShouldCollectClassSymbol(INamedTypeSymbol classSymbol)
        {
            return classSymbol.IsImplements(this.implementedInterface);
        }
    }
}