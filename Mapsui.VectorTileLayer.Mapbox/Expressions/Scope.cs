using Mapsui.VectorTileLayer.Core.Interfaces;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.MapboxGL.Expressions
{
    public class Scope
    {
        public Dictionary<string, IExpression> Bindings { get; }
        public Scope Parent { get; }

        public Scope(Dictionary<string, IExpression> bindings, Scope parent = null)
        {
            Bindings = bindings;
            Parent = parent;
        }

        public IExpression GetBinding(string name)
        {
            if (Bindings.ContainsKey(name))
            {
                return Bindings[name];
            }
            else
            {
                if (Parent != null)
                {
                    return Parent.GetBinding(name);
                }
            }

            return null;
        }
    }
}
