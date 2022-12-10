using Mapsui.Styles;

namespace Mapsui.VectorTileLayers.Core.Primitives
{
    /// <summary>
    /// Context for which the style should be evaluated
    /// </summary>
    public class EvaluationContext
    {
        public float? Zoom { get; set; }

        public float Scale { get; set; }

        public float Rotation { get; set; }

        public TagsCollection Tags { get; set; }

        public EvaluationContext(float? zoom, float scale = 1, float rotation = 0, TagsCollection tags = null)
        {
            Zoom = zoom;
            Scale = scale;
            Rotation = rotation;
            Tags = tags;
        }

        public override bool Equals(object other) => other is EvaluationContext context && Equals(context);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Zoom;
                hashCode = (hashCode * 587) ^ (int)Scale;
                hashCode = (hashCode * 587) ^ (int)Rotation;
                hashCode = (hashCode * 587) ^ (Tags != null ? Tags.GetHashCode() : 0);
                return hashCode;
            }
        }

        public bool Equals(EvaluationContext context)
        {
            return this == context || (context != null && context.Zoom == Zoom && context.Scale == Scale && ((context.Tags == null && Tags == null) || context.Tags.Equals(Tags)));
        }
    }
}
