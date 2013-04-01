﻿using System;
using System.Linq.Expressions;
using Glass.Mapper.Sc.RenderField;

namespace Glass.Mapper.Sc.Web.Ui
{
    /// <summary>
    /// Class GlassPage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GlassPage<T> : AbstractGlassPage where T : class
    {
        /// <summary>
        /// Model to render on the sublayout
        /// </summary>
        /// <value>The model.</value>
        public T Model { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlassPage{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
         public GlassPage(ISitecoreContext context) : base(context) { }
         /// <summary>
         /// Initializes a new instance of the <see cref="GlassPage{T}"/> class.
         /// </summary>
         public GlassPage() : base() { }

         /// <summary>
         /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
         /// </summary>
         /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            Model = SitecoreContext.CreateType<T>(LayoutItem, false, false);
            base.OnLoad(e);
        }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model =&gt; model.Title where Title is field name.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>System.String.</returns>
        public string Editable(Expression<Func<T, object>> field)
        {
            return base.Editable(this.Model, field);
        }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model =&gt; model.Title where Title is field name.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string Editable(Expression<Func<T, object>> field, string parameters)
        {
            return base.Editable(this.Model, field, parameters);
        }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model =&gt; model.Title where Title is field name.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string Editable(Expression<Func<T, object>> field, AbstractParameters parameters)
        {
            return base.Editable(this.Model, field, parameters);
        }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model =&gt; model.Title where Title is field name.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="standardOutput">The standard output.</param>
        /// <returns>System.String.</returns>
        public string Editable(Expression<Func<T, object>> field, Expression<Func<T, string>> standardOutput)
        {
            return base.Editable(this.Model, field, standardOutput);
        }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model =&gt; model.Title where Title is field name.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="standardOutput">The standard output.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string Editable(Expression<Func<T, object>> field, Expression<Func<T, string>> standardOutput,
                               AbstractParameters parameters)
        {
            return base.Editable(this.Model, field, standardOutput, parameters);
        }
    }

}
