using Microsoft.CodeAnalysis;

namespace DEF.CodeGenerator
{
    // Provides syntax context receivier which delegates work to multiple receivers.
    public class DerivedInterfacesReceiver : SyntaxReceiver
    {
        private string implementedInterface;

        public DerivedInterfacesReceiver(string implementedInterface) => this.implementedInterface = implementedInterface;

        public override bool CollectInterfaceSymbol { get; } = true;

        protected override bool ShouldCollectInterfaceSymbol(INamedTypeSymbol interfaceSymbol)
        {
            // return interfaceSymbol.IsDerivedFromType(this.baseTypeName);

            return interfaceSymbol.IsImplements(this.implementedInterface);
        }
    }
}