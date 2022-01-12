namespace Mapsui.VectorTileLayers.OpenMapTiles.Expressions
{
    /// <summary>
    /// Controls the annotation behavior of the parser when encountering an expression
    /// whose type is not a subtype of the expected type.The default behavior, used
    /// when optional<TypeAnnotationOption> is a nullopt, is as follows:
    /// When we expect a number, string, boolean, or array but have a value, wrap it in an assertion.
    /// When we expect a color or formatted string, but have a string or value, wrap it in a coercion.
    /// Otherwise, we do static type-checking.
    /// These behaviors are overridable for:
    /// * The "coalesce" operator, which needs to omit type annotations.
    /// * String-valued properties (e.g. `text-field`), where coercion is more convenient than assertion.
    /// </summary>
    public enum TypeAnnotationOption
    {
        None,
        Coerce,
        Assert,
        Omit,
    }
}
