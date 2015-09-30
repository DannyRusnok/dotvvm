using DotVVM.Framework.Binding;
using DotVVM.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotVVM.Framework.Controls
{
    /// <summary>
    /// Renders the HTML button which is able to trigger a postback.
    /// </summary>
    public class Button : ButtonBase
    {
        /// <summary>
        /// Gets or sets whether the control should render a submit button or a normal button (type="submit" or type="button").
        /// The submit button has some special features in the browsers, e.g. handles the Return key in HTML forms etc.
        /// </summary>
        [MarkupOptions(AllowBinding = false)]
        public bool IsSubmitButton
        {
            get { return (bool)GetValue(IsSubmitButtonProperty); }
            set { SetValue(IsSubmitButtonProperty, value); }
        }

        public static readonly DotvvmProperty IsSubmitButtonProperty
            = DotvvmProperty.Register<bool, Button>(c => c.IsSubmitButton, false);

        /// <summary>
        /// Gets or sets whether the control should render the &lt;input&gt; or the &lt;button&gt; tag in the HTML.
        /// </summary>
        [MarkupOptions(AllowBinding = false)]
        public ButtonTagName ButtonTagName
        {
            get { return (ButtonTagName)GetValue(ButtonTagNameProperty); }
            set { SetValue(ButtonTagNameProperty, value); }
        }

        public static readonly DotvvmProperty ButtonTagNameProperty
            = DotvvmProperty.Register<ButtonTagName, Button>(c => c.ButtonTagName, ButtonTagName.input);

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        public Button() : base("input")
        {
            if (ButtonTagName == ButtonTagName.button)
            {
                TagName = "button";
            }
        }

        /// <summary>
        /// Adds all attributes that should be added to the control begin tag.
        /// </summary>
        protected override void AddAttributesToRender(IHtmlWriter writer, RenderContext context)
        {
            if ((HasBinding(TextProperty) || !string.IsNullOrEmpty(Text)) && !HasOnlyWhiteSpaceContent())
            {
                throw new Exception("The <dot:Button> control cannot have both inner content and the Text property set!");     // TODO
            }

            if (ButtonTagName == ButtonTagName.button)
            {
                TagName = "button";
            }
            writer.AddAttribute("type", IsSubmitButton ? "submit" : "button");

            var clickBinding = GetCommandBinding(ClickProperty);
            if (clickBinding != null)
            {
                writer.AddAttribute("onclick", KnockoutHelper.GenerateClientPostBackScript(clickBinding, context, this));
            }

            writer.AddKnockoutDataBind(ButtonTagName == ButtonTagName.input ? "value" : "text", this, TextProperty, () =>
            {
                if (!HasOnlyWhiteSpaceContent())
                {
                    if (ButtonTagName == ButtonTagName.input)
                    {
                        throw new Exception("The <dot:Button> control cannot have inner content unless the 'ButtonTagName' property is set to 'button'!");     // TODO
                    }
                }

                if (ButtonTagName == ButtonTagName.input)
                {
                    writer.AddAttribute("value", Text);
                }
            });

            base.AddAttributesToRender(writer, context);
        }

        protected override void RenderBeginTag(IHtmlWriter writer, RenderContext context)
        {
            if (ButtonTagName == ButtonTagName.input)
            {
                writer.RenderSelfClosingTag(ButtonTagName.ToString());
            }
            else
            {
                base.RenderBeginTag(writer, context);
            }
        }

        protected override void RenderContents(IHtmlWriter writer, RenderContext context)
        {
            if (ButtonTagName == ButtonTagName.button)
            {
                if (!HasBinding(TextProperty))
                {
                    // render contents inside
                    if (!HasOnlyWhiteSpaceContent())
                    {
                        base.RenderContents(writer, context);
                    }
                    else
                    {
                        writer.WriteText(Text);
                    }
                }
            }
        }

        protected override void RenderEndTag(IHtmlWriter writer, RenderContext context)
        {
            if (ButtonTagName != ButtonTagName.input)
            {
                base.RenderEndTag(writer, context);
            }
        }
    }
}