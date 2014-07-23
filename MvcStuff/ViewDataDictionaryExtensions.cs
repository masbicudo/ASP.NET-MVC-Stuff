using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcStuff
{
    public static class ViewDataDictionaryExtensions
    {
        /// <summary>
        /// Gets an enumerable <see cref="ModelState"/>s collection from a <see cref="ViewDataDictionary"/>.
        /// </summary>
        /// <param name="viewData"><see cref="ViewDataDictionary"/> to get <see cref="ModelState"/> from.</param>
        /// <param name="excludePropertyErrors">Indicates whether to exclude property <see cref="ModelState"/>s or not.</param>
        /// <returns>An enumeration of <see cref="ModelState"/> objects.</returns>
        public static IEnumerable<ModelState> GetModelStates(this ViewDataDictionary viewData, bool excludePropertyErrors)
        {
            if (excludePropertyErrors)
            {
                ModelState modelState;
                viewData.ModelState.TryGetValue(viewData.TemplateInfo.HtmlFieldPrefix, out modelState);
                var result = modelState == null ? Enumerable.Empty<ModelState>() : SingleSet(modelState);
                return result;
            }
            else
            {
                // Model states must be returned in the order specified in the ModelMetadata.
                var result =
                    from p in viewData.ModelMetadata.Properties
                    select viewData.ModelState.GetValueOrDefault(
                        viewData.TemplateInfo.GetFullHtmlFieldName(p.PropertyName));

                return result;
            }
        }

        private static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value;
            var result = dic.TryGetValue(key, out value) ? value : defaultValue;
            return result;
        }

        private static IEnumerable<T> SingleSet<T>(T value)
        {
            yield return value;
        }
    }
}