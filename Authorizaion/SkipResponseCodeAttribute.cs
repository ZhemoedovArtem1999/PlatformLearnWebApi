namespace Authentication
{
    /// <summary>
    /// Скрывает ответ с кодом <see cref="StatusCode"/> из описания спецификации Swagger.
    /// </summary>
    /// <param name="statusCode">Код, ответы с которым скрываются из спецификации Swagger.</param>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SkipResponseCodeAttribute(int statusCode) : Attribute
    {
        /// <summary>
        /// Код, ответы с которым скрываются из спецификации Swagger.
        /// </summary>
        public int StatusCode { get; } = statusCode;
    }
}
