﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using Glass.Mapper.Profilers;
using Glass.Mapper.Sc.Profilers;
using RazorEngine.Templating;
using Sitecore.Web.UI;
using System.Web.UI;
using Sitecore.Data.Items;
using System.Web.Mvc;

namespace Glass.Mapper.Sc.Razor.Web.Ui
{
    /// <summary>
    /// Class AbstractRazorControl
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractRazorControl<T> : WebControl, IRazorControl, global::Sitecore.Layouts.IExpandable
    {
        IPerformanceProfiler _profiler = new SitecoreProfiler();
        public IPerformanceProfiler Profiler
        {
            get{
                return _profiler;
            }
            set
            {
                _profiler = value;
            }
        }

        private ISitecoreContext _sitecoreContext;

        public ViewManager ViewManager { get; private set; }

        /// <summary>
        /// A list of placeholders to render on the page.
        /// </summary>
        /// <value>The placeholders.</value>
        public IEnumerable<string> Placeholders
        {
            get;
            set;
        }

        /// <summary>
        /// View data
        /// </summary>
        /// <value>The view data.</value>
        public ViewDataDictionary ViewData { get; private set; }

        /// <summary>
        /// The path to the Razor view
        /// </summary>
        /// <value>The view.</value>
        public CachedView View
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the Glass Context to use
        /// </summary>
        /// <value>The name of the context.</value>
        public string ContextName
        {
            get;
            set;
        }

        /// <summary>
        /// The model to pass to the Razor view.
        /// </summary>
        /// <value>The model.</value>
        public T Model
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the sitecore service.
        /// </summary>
        /// <value>The sitecore service.</value>
        public ISitecoreContext SitecoreContext
        {
            get
            {
                
                if (_sitecoreContext == null)
                {
                    if (ContextName.IsNotNullOrEmpty())
                    {
                        _sitecoreContext = new SitecoreContext(ContextName)
                            {
                                Profiler = Profiler
                            };
                    }
                    else
                    {
                        _sitecoreContext = new SitecoreContext()
                        {
                            Profiler = Profiler
                        };
                    }
                }
                return _sitecoreContext;
            }
        }


       
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractRazorControl{T}"/> class.
        /// </summary>
        public AbstractRazorControl()
        {
            ViewData = new ViewDataDictionary();
            ViewManager = new ViewManager();
        }

        /// <summary>
        /// Put your logic to create your model here
        /// </summary>
        /// <returns>`0.</returns>
        public abstract T GetModel();

        /// <summary>
        /// Returns either the data source item or if no data source is specified the context item
        /// </summary>
        /// <returns>Item.</returns>
        protected Item GetDataSourceOrContextItem()
        {
            return this.DataSource.IsNullOrEmpty() ? global::Sitecore.Context.Item :
                global::Sitecore.Context.Database.GetItem(this.DataSource);
        }

        /// <summary>
        /// Get caching identifier. Must be implemented by controls that supports caching.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>If an empty string is returned, the control will not be cached.</remarks>
        protected override string GetCachingID()
        {
            return this.View.Name;
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="output">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
        /// <exception cref="Glass.Mapper.Sc.Razor.RazorException"></exception>
        /// <remarks>When developing custom server controls, you can override this method to generate content for an ASP.NET page.</remarks>
        protected override void DoRender(HtmlTextWriter output)
        {
           Profiler.Start("Get Model");

            Model = GetModel();

            Profiler.End("Get Model");

            try
            {
                Profiler.Start("Razor engine {0}".Formatted(this.View.Name));

                Profiler.Start("Create Template");

                var template =
                    RazorEngine.Razor.GetTemplate<T>(View.ViewContent, Model, View.Name) as ITemplateBase;
              
                Profiler.End("Create Template");

                Profiler.Start("Configure Template");

                template.Configure(SitecoreContext, ViewData, this);

                Profiler.End("Configure Template");

                Profiler.Start("Run Template");

                output.Write( ((RazorEngine.Templating.ITemplate)template).Run(new ExecuteContext()));

                Profiler.End("Run Template");

                Profiler.End("Razor engine {0}".Formatted(this.View));

            }
            catch (RazorEngine.Templating.TemplateCompilationException ex)
            {
                StringBuilder errors = new StringBuilder();
                ex.Errors.ForEach(x =>
                                      {
                                          errors.AppendLine("File: {0}".Formatted(View));
                                          errors.AppendLine(x.ErrorText);
                                      });


             //   throw new RazorException(errors.ToString());

                WriteException(output, ex);
            }
            catch (Exception ex)
            {
                WriteException(output, ex);
            }
        }

        private void WriteException(HtmlTextWriter output, Exception ex)
        {
            output.Write("<h1>Glass Razor Rendering Exception</h1>");
            output.Write("<p>View: {0}</p>".Formatted(this.View));
            output.Write("<p>{0}</p>".Formatted(ex.Message));
            output.Write("<pre>{0}</pre>".Formatted(ex.StackTrace));
        
            Sitecore.Diagnostics.Log.Error("Glass Razor Rendering Error {0}".Formatted(this.View), ex, this);

        }

        /// <summary>
        /// Expands this instance.
        /// </summary>
        public void Expand()
        {
            if (Placeholders != null)
            {
                foreach (var placeHolderName in Placeholders)
                {
                    global::Sitecore.Web.UI.WebControls.Placeholder holder = new global::Sitecore.Web.UI.WebControls.Placeholder();
                    holder.Key = placeHolderName.ToLower();
                    this.Controls.Add(holder);
                }
            }

            this.Controls.Cast<Control>().Where(x => x is global::Sitecore.Layouts.IExpandable)
                .Cast<global::Sitecore.Layouts.IExpandable>().ToList().ForEach(x => x.Expand());
        }
    }
   
}
