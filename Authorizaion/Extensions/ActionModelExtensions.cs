using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Authentication.Extensions
{
    internal static class ActionModelExtensions
    {
        /// <summary>
        /// Проверяет <see cref="ActionModel"/> или вышестоящий контроллер на наличие атрибутов <see cref="ProducesResponseTypeAttribute"/> или <see cref="SkipResponseCodeAttribute"/> с кодом ответа <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="self"><see cref="ActionModel"/> для проверки.</param>
        /// <param name="statusCode">Код ответа для проверки.</param>
        /// <returns><see langword="true"/> если <see cref="ActionModel"/> или вышестоящий контроллер содержит атрибут с кодом ответа <paramref name="statusCode"/>.</returns>
        public static bool HasResponseCode(this ActionModel self, int statusCode)
        {
            if (self is not null)
            {
                if (self.Filters.Any(f => f is ProducesResponseTypeAttribute responseFilter && responseFilter.StatusCode == statusCode))
                    return true;

                if (self.Attributes.Any(a => a is SkipResponseCodeAttribute attribute && attribute.StatusCode == statusCode)) 
                    return true;

                if (self.Controller.Attributes.Any(a => a is SkipResponseCodeAttribute attribute && attribute.StatusCode == statusCode))
                    return true;

                if (self.Controller.Attributes.Any(a => a is ProducesResponseTypeAttribute attribute && attribute.StatusCode == statusCode))
                    return true;
            }

            return false;
        }
    }
}
