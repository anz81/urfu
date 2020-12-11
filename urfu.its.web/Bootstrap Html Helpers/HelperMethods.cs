using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
//using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Encodings.Web;
//using System.Web.Helpers;
//using System.Web.UI;
//using System.Web.UI.HtmlControls;
using Urfu.Its.Common;

namespace Microsoft.AspNetCore.Mvc.Html
{
    /// <summary>
    /// This class have methods that will help to manipulate HTML Helpers.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Method that will execute Attributes.Add for each given value in the attributes, then insert them into the specified tag.
        /// </summary>
        /// <param name="Tag">The tag to apply the attributes.</param>
        /// <param name="Attributes">The attributes to be added to the tag. Attention: Separate the key from the value with an equal (=) character.</param>
        public static void AddAtrributes(TagBuilder Tag, params string[] Attributes)
        {
            try
            {
                // Adds all other attributes.
                foreach (var attr in Attributes)
                {
                    Tag.Attributes.Add(attr.Split('=')[0], attr.Split('=')[1]);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This will get the current Culture specified under the system.web/globalization in the Web.Config.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetCurrentCulture()
        {
            try
            {
                // Get the globalization section under system.web in Web.Config.
                IList<CultureInfo> supportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("ru-RU") };

                // If the section doesn't exist in the Web.Config, returns null.
                
                return new CultureInfo("ru-RU");
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        /// <summary>
        /// Method that will search for the Validation properties in the metadata, then insert them into the specified tag.
        /// </summary>
        /// <param name="self">The helper itself.</param>
        /// <param name="expression">Model's expression.</param>
        /// <param name="metadata">The metadata from the model.</param>
        /// <param name="tag">The tag to be validated.</param>
        public static void AddValidationProperties<TModel, TValue>(HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression, TagBuilder tag)
        {
            // Get MetaData.
            //    var metadata = ModelExpressionProvider.CreateModelExpression(self.ViewData, expression).Metadata;
            //var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var mep = new ModelExpressionProvider(new EmptyModelMetadataProvider());
            var metadata = mep.CreateModelExpression(self.ViewData, expression).Metadata;
            var me = new ModelExplorer(new EmptyModelMetadataProvider(), metadata, self);
            // Do stuff.
            var fieldName = mep.GetExpressionText(expression);
            var fullBindingName = self.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var fieldId = TagBuilder.CreateSanitizedId(fullBindingName, "-");
            // Add the validation data-* properties to the tag.
            var generator = self.ViewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var validationAttributes = MyHtmlGenerator.AddValidationAttributes(self.ViewContext, tag, me, expression);
                //fullBindingName, metadata);
            foreach (var key in validationAttributes.Keys)
            {
                tag.Attributes.Add(key, validationAttributes[key].ToString());
            }
        }

     
    }
    public class MyHtmlGenerator : DefaultHtmlGenerator
    {
        public MyHtmlGenerator(
            IAntiforgery antiforgery,
            IOptions<MvcViewOptions> optionsAccessor,
            IModelMetadataProvider metadataProvider,
            IUrlHelperFactory urlHelperFactory,
            HtmlEncoder htmlEncoder,
            ValidationHtmlAttributeProvider clientValidatorCache)
            : base(
                  antiforgery,
                  optionsAccessor,
                  metadataProvider,
                  urlHelperFactory,
                  htmlEncoder,
                  clientValidatorCache)
        {
        }

        public void AddValidationAttributes<TModel, TValue>(ViewContext viewContext, TagBuilder tag, ModelExplorer mep , Expression<Func<TModel, TValue>> expression)
        {
            return DefaultHtmlGenerator.AddValidationAttributes(viewContext, tag, mep, expression);
        }

    }
}