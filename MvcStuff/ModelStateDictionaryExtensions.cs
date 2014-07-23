using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace MvcStuff
{
    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Adds a state error related to a model property.
        /// </summary>
        /// <param name="modelStateDictionary"><see cref="ModelStateDictionary"/> to add the model property error to.</param>
        /// <param name="lambdaExpression"><see cref="Expression"/> referencing the property that owns the model error.</param>
        /// <param name="error">Error message indicating what is wrong.</param>
        public static void AddModelError(
            this ModelStateDictionary modelStateDictionary,
            Expression<Func<object>> lambdaExpression,
            [Localizable(true)] string error)
        {
            var key = ExpressionHelper.GetExpressionText(lambdaExpression);
            modelStateDictionary.AddModelError(key, error);
        }

        /// <summary>
        /// Flatten all model errors in a single list of tuples containing the property name and the ModelError object.
        /// </summary>
        /// <param name="modelState">ModelStateDictionary to flatten.</param>
        /// <returns>A single flattened list of all model errors.</returns>
        public static List<Tuple<string, ModelError>> GetAllErrors(this ModelStateDictionary modelState)
        {
            var result = new List<Tuple<string, ModelError>>();

            foreach (var eachModelState in modelState)
                foreach (var eachModelError in eachModelState.Value.Errors)
                    result.Add(new Tuple<string, ModelError>(eachModelState.Key, eachModelError));

            return result;
        }

#if !DISABLE_SPECIFICS
        /// <summary>
        /// Gets a lines of a markdown formatted text with all errors in the <see cref="ModelStateDictionary"/>.
        /// </summary>
        /// <param name="modelState"><see cref="ModelStateDictionary"/> to get model errors from.</param>
        /// <param name="title">Top title of the markdown text.</param>
        /// <returns>An array of markdown text lines containing all model errors.</returns>
        public static string[] GetMarkdownErrorLines(
            this ModelStateDictionary modelState,
            [Localizable(true)] string title)
        {
            var lines = new[] { title, new string('=', title.Length), }
                .Concat(
                    modelState.GetAllErrors()
                        .SelectMany(
                            x => new[]
                            {
                                x.Item1,
                                new string('-', x.Item1.Length),
                                x.Item2.ErrorMessage
                            }))
                .ToArray();

            return lines;
        }

        public static void FillJsonResponse(
            this ModelStateDictionary modelState,
            JsonResponseData target,
            string title,
            string errorType)
        {
            if (!modelState.IsValid)
            {
                target.Message = string.Join("\n", modelState.GetMarkdownErrorLines(title));
                target.Success = false;
                target.ModelErrors = modelState.GetJsonModelErrors();
                target.ErrorType = errorType;
            }
        }

        public static JsonModelErrorData[] GetJsonModelErrors(
            this ModelStateDictionary modelState)
        {
            var errors = modelState.GetAllErrors()
                .GroupBy(x => x.Item2.ErrorMessage)
                .Select(
                    x => new JsonModelErrorData
                    {
                        Members = x.Select(y => y.Item1).ToArray(),
                        Message = x.Key,
                    })
                .ToArray();

            return errors;
        }
#endif
    }
}