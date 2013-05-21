﻿using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Data;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.GetModel;
using Sitecore.Mvc.Presentation;

namespace Glass.Mapper.Sc.Pipelines.Response
{
    /// <summary>
    /// 
    /// </summary>
    public class GetModel : GetModelProcessor
    {

        /// <summary>
        /// The model type field
        /// </summary>
        public const string ModelTypeField = "Model Type";

        /// <summary>
        /// The model field
        /// </summary>
        public const string ModelField = "Model";

        /// <summary>
        /// Initializes a new instance of the <see cref="GetModel"/> class.
        /// </summary>
        public GetModel()
        {
            ContextName = "Default";

        }

        /// <summary>
        /// Gets or sets the name of the context.
        /// </summary>
        /// <value>
        /// The name of the context.
        /// </value>
        public string ContextName { get; set; }

        /// <summary>
        /// Processes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process(GetModelArgs args)
        {
            if (args.Result == null)
            {
                Rendering rendering = args.Rendering;
                if (rendering.RenderingType == "Layout")
                {
                    args.Result = GetFromItem(rendering, args);
                    if (args.Result == null)
                    {
                        args.Result = GetFromLayout(rendering, args);
                    }
                }
                if (args.Result == null)
                {
                    args.Result = GetFromPropertyValue(rendering, args);
                }
                if (args.Result == null)
                {
                    args.Result = GetFromField(rendering, args);
                }
            }
        }

        /// <summary>
        /// Gets from field.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        protected virtual object GetFromField(Rendering rendering, GetModelArgs args)
        {
            Item obj = ObjectExtensions.ValueOrDefault<RenderingItem, Item>(rendering.RenderingItem, (Func<RenderingItem, Item>)(i => i.InnerItem));
            if (obj == null)
                return (object)null;
            else
                return GetObject(obj[ModelField], rendering.Item.Database);
        }

        /// <summary>
        /// Gets from property value.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        protected virtual object GetFromPropertyValue(Rendering rendering, GetModelArgs args)
        {
            string model = rendering.Properties[ModelField];
            if (StringExtensions.IsWhiteSpaceOrNull(model))
                return (object)null;
            else
                return GetObject(model, rendering.Item.Database);
        }

        /// <summary>
        /// Gets from layout.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        protected virtual object GetFromLayout(Rendering rendering, GetModelArgs args)
        {
            string pathOrId = rendering.Properties["LayoutId"];
            if (StringExtensions.IsWhiteSpaceOrNull(pathOrId))
                return (object)null;
            string model = ObjectExtensions.ValueOrDefault<Item, string>(MvcSettings.GetRegisteredObject<ItemLocator>().GetItem(pathOrId), (Func<Item, string>)(i => i["Model"]));
            if (StringExtensions.IsWhiteSpaceOrNull(model))
                return (object) null;
            else
                return GetObject(model, rendering.Item.Database);
        }

        /// <summary>
        /// Gets from item.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        protected virtual object GetFromItem(Rendering rendering, GetModelArgs args)
        {
            string model = ObjectExtensions.ValueOrDefault<Item, string>(rendering.Item, (Func<Item, string>)(i => i["MvcLayoutModel"]));
            if (StringExtensions.IsWhiteSpaceOrNull(model))
                return (object) null;
            else
                return GetObject(model, rendering.Item.Database);
        }


        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="db">The db.</param>
        /// <returns></returns>
        /// <exception cref="Glass.Mapper.MapperException">Failed to find context {0}.Formatted(ContextName)</exception>
        public object GetObject(string model, Database db)
        {

            if (model.IsNullOrEmpty())
                return null;

            //must be a path to a Model item
            if (model.StartsWith("/sitecore"))
            {
                var target = db.GetItem(model);
                if (target == null)
                    return null;

                string newModel = target[ModelTypeField];
                return GetObject(newModel, db);
            }
            //if guid must be that to Model item
            Guid targetId;
            if (Guid.TryParse(model, out targetId))
            {
                var target = db.GetItem(new ID(targetId));
                if (target == null)
                    return null;

                string newModel = target[ModelTypeField];
                return GetObject(newModel, db);
            }

            var type = Type.GetType(model, true);

            if (type == null)
                return null;

            var context = Context.Contexts[ContextName];
            if (context == null) throw new MapperException("Failed to find context {0}".Formatted(ContextName));

            if (context.TypeConfigurations.ContainsKey(type))
            {
                ISitecoreContext scContext = new SitecoreContext(context);
                var result = scContext.GetCurrentItem(type);
               return result;
            }
            return null;
        }

        
      
    }
}
