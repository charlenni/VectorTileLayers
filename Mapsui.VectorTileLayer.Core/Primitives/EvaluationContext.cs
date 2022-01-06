namespace Mapsui.VectorTileLayer.Core.Primitives
{
    /// <summary>
    /// Context for which the style should be evaluated
    /// </summary>
    public class EvaluationContext
    {
        public float? Zoom { get; set; }

        public float Scale { get; set; }

        public TagsCollection Tags { get; set; }

        public EvaluationContext(float? zoom, float scale = 1, TagsCollection tags = null)
        {
            Zoom = zoom;
            Scale = scale;
            Tags = tags;
        }

        public override bool Equals(object other) => other is EvaluationContext context && Equals(context);

        public bool Equals(EvaluationContext context)
        {
            return context != null && context.Zoom == Zoom && context.Scale == Scale && ((context.Tags == null && Tags == null) || context.Tags.Equals(Tags));
        }
    }
}
